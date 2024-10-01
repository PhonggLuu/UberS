using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberSystem.Dto.Requests
{
	public class RatingRequest
	{
		public long TripId { get; set; }

		public int Rating1 { get; set; }

		public string? Feedback { get; set; }
	}
}
