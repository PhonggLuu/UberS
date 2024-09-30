using UberSystem.Domain.Entities;

namespace UberSystem.Domain.Interfaces.Services
{
	public interface ILocateService
	{
		Task ReadExcelFile(string filePath);
		Task GenerateData(List<Locate> locates);
		Task<Locate> GetCoordinates(string pickUpAddress, string pickUpWard);
	}
}
