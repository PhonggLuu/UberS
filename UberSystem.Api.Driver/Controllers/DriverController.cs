using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UberSystem.Domain.Enums;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Dto.Requests;

namespace UberSystem.Api.Driver.Controllers
{
	[Route("api/driver")]
	[ApiController]
	public class DriverController : ControllerBase
	{
		private readonly IDriverService _driverService;
		private readonly IMapper _mapper;

		public DriverController(IDriverService driverService, IMapper mapper)
		{
			_driverService = driverService;
			_mapper = mapper;
		}

		[HttpPut]
		public async Task<IActionResult> UpdateStatus([FromQuery] Status status,
													[FromBody] TripRequest request)
		{
			try
			{
				await _driverService.UpdateStatus(request.DriverId, request.CustomerId, request.PaymentId, request.Status, request.PickUpAddress,
															request.PickUpWard, request.DropOffAddress, request.DropOffWard, status);
			} catch (Exception e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
			return Ok();
		}
	}
}
