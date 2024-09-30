namespace UberSystem.Domain.Interfaces.Services
{
	public interface IRatingService
	{
		Task<double?> CalculateRating(long driverId);
	}
}
