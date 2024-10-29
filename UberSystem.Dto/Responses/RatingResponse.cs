namespace UberSystem.Dto.Responses
{
	public class RatingResponse
	{
		public long Id { get; set; }

		public long? CustomerId { get; set; }

		public long? DriverId { get; set; }

		public long? TripId { get; set; }

		public int? Rating { get; set; }

		public string? Feedback { get; set; }
	}
}
