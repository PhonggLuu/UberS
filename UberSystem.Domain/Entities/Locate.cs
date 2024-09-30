namespace UberSystem.Domain.Entities
{
	public class Locate
	{
		public int Id { get; set; }
		public double StartLatitude { get; set; }
		public double StartLongitude { get; set; }
		public double TempLatitude { get; set; }
		public double TempLongitude { get; set; }
		public double EndLatitude { get; set; }
		public double EndLongitude { get; set; }
		public string? StartAddress { get; set; }
		public string? StartWard { get; set; }
		public string? TempAddress { get; set; }
		public string? TempWard { get; set; }
		public string? EndAddress { get; set; }
		public string? EndWard { get; set; }
	}
}
