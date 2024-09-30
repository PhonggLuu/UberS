namespace UberSystem.Dto.Requests
{
	public class TripRequest
	{

		public long CustomerId { get; set; }

		public long DriverId { get; set; }

		public long? PaymentId { get; set; }

		public string? Status { get; set; }

		public required string PickUpAddress { get; set; }

		public required string PickUpWard { get; set; }

		public required string DropOffAddress { get; set; }

		public required string DropOffWard { get; set; }
	}
}
