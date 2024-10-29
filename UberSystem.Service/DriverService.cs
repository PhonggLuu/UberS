using UberSystem.Domain.Entities;
using UberSystem.Common.Enums;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;
using UberSytem.Dto;
using UberSystem.Dto.Requests;

namespace UberSystem.Service
{
	public class DriverService : IDriverService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRatingService _ratingService;
		private readonly ITripService _tripService;
		private readonly ILocateService _locateService;

		public DriverService(IUnitOfWork unitOfWork, IRatingService ratingService, ITripService tripService, ILocateService locateService)
		{
			_unitOfWork = unitOfWork;
			_ratingService = ratingService;
			_tripService = tripService;
			_locateService = locateService;
		}
		public async Task<Driver?> GetDriversHighRating(double pickUplatitude, double pickUplongitude)
		{
			IEnumerable<Driver>? drivers = await GetNearestDrivers(pickUplatitude, pickUplongitude);
			double maxRating = 0;
			if(drivers is null)
				return null;
			Driver? response = drivers.FirstOrDefault();
			foreach (var driver in drivers)
			{
				double? rating = await _ratingService.CalculateRating(driver.Id);
				if(rating > maxRating && driver.Status != Status.TEMPORARILY_LOCKED && driver.Status != Status.OFFLINE)
				{
					maxRating = rating.Value;
					response = driver;
				}
			}
			return response;
		}
		private async Task<IEnumerable<Driver>?> GetNearestDrivers(double pickUplatitude, double pickUplongitude)
		{
			var driverRepository = _unitOfWork.Repository<Driver>();

			IEnumerable<Driver> drivers = await driverRepository.GetAllAsync();
			if(drivers is null) 
			{
				return null;
			}

			drivers = drivers
					.Where(d => d.LocationLatitude.HasValue && d.LocationLongitude.HasValue &&
								GetDistance(d.LocationLatitude.Value, d.LocationLongitude.Value, pickUplatitude, pickUplongitude) <= 2 && d.Status == Status.FREE);
			return drivers;
		}

		//locate là điểm mà driver đang đứng
		//pick up là điểm mà customer đang đứng
		private double GetDistance(double locateLatitude, double locateLongitude, double pickUplatitude, double pickUplongitude)
		{
			const double EarthRadius = 6371.0; // Bán kính trái đất tính bằng km

			// Chuyển đổi độ sang radian
			double lat1Rad = DegreesToRadians(locateLatitude);
			double lat2Rad = DegreesToRadians(pickUplatitude);
			double deltaLatRad = DegreesToRadians(pickUplatitude - locateLatitude);
			double deltaLonRad = DegreesToRadians(pickUplongitude - locateLongitude);

			// Công thức Haversine
			double a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
					   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
					   Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

			// Tính khoảng cách
			double distance = EarthRadius * c;

			return distance;
		}

		private double DegreesToRadians(double degrees)
		{
			return degrees * Math.PI / 180.0;
		}

		public async Task GenerateDriverData() { 			
			var driverRepository = _unitOfWork.Repository<Driver>();
			for (int i = 0; i < 10; i++)
			{
				var driver = new Driver
				{
					Id = Helper.GenerateRandomLong(),
					Status = Status.FREE,
					LocationLatitude = 106.74 + i / 100,
					LocationLongitude = 106.74 + i/100,
				};
				await driverRepository.InsertAsync(driver);
			}
			for (int i = 0; i < 5; i++)
			{
				var driver = new Driver
				{
					Id = Helper.GenerateRandomLong(),
					Status = Status.INPROGRESS,
					LocationLatitude = 106.65 + i / 100,
					LocationLongitude = 106.61 + i / 100,
				};
				await driverRepository.InsertAsync(driver);
			}
		}

		public async Task UpdateStatus(long driverId, long customerId, long? paymentId, string? status1, string pickUpAddress, string pickUpWard, string dropOffAddress, string dropOffWard, Status status2)
		{

			var driverRepository = _unitOfWork.Repository<Driver>();
			var driver = await driverRepository.FindAsync(driverId);
			if(driver is null)
				throw new KeyNotFoundException("Driver not found.");
			driver.Status = status2;
			await driverRepository.UpdateAsync(driver);

			var sourceLocate = await _locateService.GetCoordinates(pickUpAddress, pickUpWard);
			var destinationLocate = await _locateService.GetCoordinates(dropOffAddress, dropOffWard);

			var trip = new Trip
			{
				Id = Helper.GenerateRandomLong(),
				DriverId = driverId,
				CustomerId = customerId,
				PaymentId = paymentId,
				Status = status1,
				SourceLatitude = sourceLocate.StartLatitude,
				SourceLongitude = sourceLocate.StartLongitude,
				DestinationLatitude = destinationLocate.StartLatitude,
				DestinationLongitude = destinationLocate.StartLongitude
			};
			await _tripService.AddNewTrip(trip);
		}

		public async Task UpdateStatus2(long driverId, Status newStatus)
		{

			var driverRepository = _unitOfWork.Repository<Driver>();
			var driver = await driverRepository.FindAsync(driverId);
			if (driver is null)
				throw new KeyNotFoundException("Driver not found.");
			driver.Status = newStatus;
			await driverRepository.UpdateAsync(driver);
		}
	}
}
