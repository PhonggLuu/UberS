using UberSystem.Domain.Entities;
using UberSystem.Domain.Enums;

namespace UberSystem.Domain.Interfaces.Services
{
	public interface IDriverService
	{
		Task<Driver?> GetDriversHighRating(double pickUplatitude, double pickUplongitude);
		Task GenerateDriverData();
		Task UpdateStatus(long driverId, long customerId, long? paymentId, string? status1, string pickUpAddress, string pickUpWard, string dropOffAddress, string dropOffWard, Status status2);
	}
}
