using UberSystem.Domain.Enums;

namespace UberSystem.Dto.Responses
{
	public class DriverResponse
	{
		public long Id { get; set; }

		public double? LocationLatitude { get; set; }

		public double? LocationLongitude { get; set; }

		public Status Status { get; set; }
	}
}
