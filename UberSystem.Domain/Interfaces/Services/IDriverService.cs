using UberSystem.Common.Enums;
using UberSystem.Domain.Entities;

namespace UberSystem.Domain.Interfaces.Services
{
	public interface IDriverService
	{
		Task<List<Driver>> GetDriversHighRating(double pickUplatitude, double pickUplongitude);
		Task GenerateDriverData();
		Task UpdateStatus(long driverId, long customerId, long? paymentId, string? status1, string pickUpAddress, string pickUpWard, string dropOffAddress, string dropOffWard, Status status2);
		Task<bool> UpdateStatus2(long driverId, Status status2);
	}
}
