using UberSystem.Domain.Entities;

namespace UberSystem.Domain.Interfaces.Services
{
	public interface IRatingService
	{
		Task<double?> CalculateRating(long driverId);
		Task RateDriver(long tripId, int rate, string? feedback);
		Task<Rating?> GetRatingAsync(long tripId);
	}
}
