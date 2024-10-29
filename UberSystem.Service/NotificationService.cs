using System.Net.Http.Json;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Dto.Requests;

namespace UberSystem.Service
{
	public class NotificationService : Domain.Interfaces.Services.INotificationService
	{

		public async Task<NotificationMessage> SendNotificationToDrivers(long driverId, CustomerLocation customerLocation)
		{
			var message = new NotificationMessage
			{
				Title = "New Ride Request",
				Body = $"You have a new ride request from {customerLocation.PickUpAddress} in {customerLocation.PickUpWard}.",
				Location = customerLocation,
			};
			return message;
		}
	}
}
