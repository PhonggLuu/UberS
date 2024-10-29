namespace UberSystem.Dto.Responses
{
    public class TripResponse
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public long DriverId { get; set; }

        public long? PaymentId { get; set; }

        public string? Status { get; set; }

        public double? SourceLatitude { get; set; }

        public double? SourceLongitude { get; set; }

        public double? DestinationLatitude { get; set; }

        public double? DestinationLongitude { get; set; }
    }
}
