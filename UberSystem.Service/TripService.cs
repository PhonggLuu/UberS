using UberSystem.Domain.Entities;
using UberSystem.Domain.Enums;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;

namespace UberSystem.Service
{
	public class TripService : ITripService
	{
		private readonly IUnitOfWork _unitOfWork;

		public TripService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task AddNewTrip(Trip trip)
		{
			try
			{
				var tripRepository = _unitOfWork.Repository<Trip>();
				if(trip is not null)
				{

					await _unitOfWork.BeginTransaction();
					await tripRepository.InsertAsync(trip);
					await _unitOfWork.CommitTransaction();
				}
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransaction();
				throw;
			}
		}


	}
}
