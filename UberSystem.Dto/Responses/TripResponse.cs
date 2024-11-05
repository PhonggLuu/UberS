using Newtonsoft.Json;
using System.Numerics;
using UberSytem.Dto;

namespace UberSystem.Dto.Responses
{
    public class TripResponse
    {
        public string Id { get; set; }
		public string CustomerId { get; set; }
		public string DriverId { get; set; }

        public long? PaymentId { get; set; }

        public string? Status { get; set; }

        public double? SourceLatitude { get; set; }

        public double? SourceLongitude { get; set; }

        public double? DestinationLatitude { get; set; }

        public double? DestinationLongitude { get; set; }
    }
}
