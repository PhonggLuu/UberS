using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;
using UberSytem.Dto;

namespace UberSystem.Service
{
	public class RatingService : IRatingService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ITripService _tripService;
		public RatingService(IUnitOfWork unitOfWork, ITripService tripService)
		{
			_unitOfWork = unitOfWork;
			_tripService = tripService;
		}
		public async Task<double?> CalculateRating(long driverId)
		{
			var ratings = await _unitOfWork.Repository<Rating>().GetAllAsync();

			var averageRating = ratings
				.Where(r => r.DriverId == driverId)
				.Average(r => (double?)r.Rating1);

			return averageRating;
		}

		public async Task RateDriver(long tripId, int rate, string? feedback)
		{
			var ratingRepository = _unitOfWork.Repository<Rating>();
			var trip = await _tripService.GetTripById(tripId);
			var rating = new Rating
			{
				Id = Helper.GenerateRandomLong(),
				DriverId = trip.DriverId,
				CustomerId = trip.CustomerId,
				TripId = tripId,
				Rating1 = rate,
				Feedback = feedback
			};
			await ratingRepository.InsertAsync(rating);
		}

		public async Task<Rating?> GetRatingAsync(long tripId)
		{
			var ratingRepository = _unitOfWork.Repository<Rating>();
			var rating = await ratingRepository.GetAsync(r => r.TripId == tripId);
			return rating;
		}
	}
}
