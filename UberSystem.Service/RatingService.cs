using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;

namespace UberSystem.Service
{
	public class RatingService : IRatingService
	{
		private readonly IUnitOfWork _unitOfWork;
		public RatingService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<double?> CalculateRating(long driverId)
		{
			var ratings = await _unitOfWork.Repository<Rating>().GetAllAsync();

			var averageRating = ratings
				.Where(r => r.DriverId == driverId)
				.Average(r => (double?)r.Rating1);

			return averageRating;
		}
	}
}
