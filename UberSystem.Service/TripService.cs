using UberSystem.Domain.Entities;
using UberSystem.Common.Enums;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;

namespace UberSystem.Service
{
	public class TripService : ITripService
	{
		private readonly IUnitOfWork _unitOfWork;

		public TripService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task AddNewTrip(Trip trip)
		{
			try
			{
				var tripRepository = _unitOfWork.Repository<Trip>();
				if(trip is not null)
				{

					await _unitOfWork.BeginTransaction();
					await tripRepository.InsertAsync(trip);
					await _unitOfWork.CommitTransaction();
				}
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransaction();
				throw;
			}
		}

		public async Task<bool> UpdateTrip(long tripId, long driverId, string? status)
		{
			try
			{
				var tripRepository = _unitOfWork.Repository<Trip>();
				var trip = await tripRepository.FindAsync(tripId);
				if (trip is not null)
				{
					await _unitOfWork.BeginTransaction();
					if(status != null)
						trip.Status = status;
					trip.DriverId = driverId;
					var updated = await tripRepository.UpdateAsync(trip);
					await _unitOfWork.CommitTransaction();
					return updated;
				}
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransaction();
				throw;
			}
			return false;
		}

		public async Task<Trip> GetTripById(long tripId)
		{
			var tripRepository = _unitOfWork.Repository<Trip>();
			var trip = await tripRepository.FindAsync(tripId);
			return trip;
		}

		public async Task<Trip?> GetCustomerTripPending(long customerId)
		{
			var tripRepository = _unitOfWork.Repository<Trip>();
			var trip = await tripRepository.GetAsync(t => t.CustomerId == customerId && t.Status == OrderStatus.PENDING.ToString());
			return trip;
		}
	}
}
