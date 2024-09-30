using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Dto.Requests;
using UberSystem.Dto.Responses;
using UberSystem.Service;

namespace UberSystem.Api.Customer.Controllers
{

	[Route("api/customer")]
	[ApiController]
	public class CustomerController : ControllerBase
	{
		private readonly IDriverService _driverService;
		private readonly ILocateService _locateService;
		private readonly IMapper _mapper;

		public CustomerController(IDriverService driverService, ILocateService locateService, IMapper mapper)
		{
			_driverService = driverService;
			_locateService = locateService;
			_mapper = mapper;
		}

		[HttpPost]
		public async Task<IActionResult> GetNearestDrivers([FromBody] LocationRequest request)
		{
			var location = await _locateService.GetCoordinates(request.PickUpAddress, request.PickUpWard);
			var driver = await _driverService.GetDriversHighRating(location.StartLatitude, location.StartLongitude);
			var response = _mapper.Map<DriverResponse>(driver);
			return Ok(response);
		}
	}
}
