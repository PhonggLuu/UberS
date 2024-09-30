using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UberSystem.Domain.Entities;

namespace UberSystem.Domain.Interfaces.Services
{
	public interface ITripService
	{
		Task AddNewTrip(Trip trip);
	}
}
