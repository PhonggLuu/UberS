// See https://aka.ms/new-console-template for more information
using UberSystem.Domain.Interfaces;
using UberSystem.Infrastructure;
using UberSystem.Service;

class Program
{
	/*static async Task Main()
	{
		// Cấu hình DbFactory và UnitOfWork
		var dbFactory = new DbFactory(() => new UberSystemDbContext(*//* options *//*));
		var unitOfWork = new UnitOfWork(dbFactory);
		*//*var locateService = new LocateService(unitOfWork);
		await locateService.ReadExcelFile("C:\\Users\\phong pc\\Desktop\\data.xlsx");*/
		/*var driverService = new DriverService(unitOfWork);
		await driverService.GenerateDriverData();
		var customerService = new CustomerService(unitOfWork);
		await customerService.GenerateCustomerData();*//*

		Console.WriteLine("Import data done");
	}*/
}
