using UberSystem.Dto.Requests;

namespace UberSystem.Domain.Interfaces.Services
{
	public interface INotificationService
	{
		Task<NotificationMessage> SendNotificationToDrivers(long driverId, CustomerLocation request);
	}
}
