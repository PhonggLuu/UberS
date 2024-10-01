namespace UberSystem.Domain.Interfaces.Services
{
	public interface IRatingService
	{
		Task<double?> CalculateRating(long driverId);
		Task RateDriver(long tripId, int rate, string? feedback);
	}
}
