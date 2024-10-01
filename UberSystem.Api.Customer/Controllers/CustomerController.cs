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
		private readonly IRatingService _ratingService;
		private readonly IMapper _mapper;

		public CustomerController(IDriverService driverService, ILocateService locateService, IRatingService ratingService, IMapper mapper)
		{
			_driverService = driverService;
			_locateService = locateService;
			_ratingService = ratingService;
			_mapper = mapper;
		}

		/// <summary>
		/// Tìm kiếm tài xế gần có rating cao nhất
		/// </summary>
		/// <returns></returns>
		[HttpPost("get-driver")]
		public async Task<IActionResult> GetNearestDrivers([FromBody] LocationRequest request)
		{
			var location = await _locateService.GetCoordinates(request.PickUpAddress, request.PickUpWard);
			var driver = await _driverService.GetDriversHighRating(location.StartLatitude, location.StartLongitude);
			var response = _mapper.Map<DriverResponse>(driver);
			return Ok(response);
		}

		/// <summary>
		/// Đánh giá tài xế
		/// </summary>
		/// <returns></returns>
		[HttpPost("rating")]
		public async Task<IActionResult> RateDriver([FromBody] RatingRequest request)
		{
			try
			{
				await _ratingService.RateDriver(request.TripId, request.Rating1, request.Feedback);
			} catch (Exception)
			{
				return BadRequest();
			}
			return Ok();
		}
	}
}
