using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;
using UberSytem.Dto;

namespace UberSystem.Service
{
	public class CustomerService : ICustomerService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILocateService _locateService;

		public CustomerService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public CustomerService(IUnitOfWork unitOfWork, ILocateService locateService)
		{
			_unitOfWork = unitOfWork;
			_locateService = locateService;
		}

		public async Task GenerateCustomerData()
		{
			var driverRepository = _unitOfWork.Repository<Customer>();
			for (int i = 0; i < 10; i++)
			{
				var driver = new Customer
				{
					Id = Helper.GenerateRandomLong(),
				};
				await driverRepository.InsertAsync(driver);
			}
		}
	}
}
