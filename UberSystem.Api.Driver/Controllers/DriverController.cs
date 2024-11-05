using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Net;
using System.Numerics;
using UberSystem.Common.Enums;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Dto.Responses;
using UberSytem.Dto;

namespace UberSystem.Api.Driver.Controllers
{
	//[Authorize(Roles = "Driver")]
	[Route("odata/driver")]
	[ApiController]
	public class DriverController : ODataController
	{
		private readonly IDriverService _driverService;
		private readonly ITripService _tripService;
		private readonly IMapper _mapper;

		public DriverController(IDriverService driverService, ITripService tripService, IMapper mapper)
		{
			_driverService = driverService;
			_tripService = tripService;
			_mapper = mapper;
		}

		/// <summary>
		/// Cập nhật trạng thái của tài xế
		/// </summary>
		/// <param name="status">Driver's status</param>
		/// <param name="confirmation">Xác nhận đơn hàng</param>
		/// <param name="request">Thông tin chuyến đi</param>
		/*[HttpPut("{driverId}")]
		public async Task<IActionResult> UpdateStatus([FromODataUri] long driverId, 
														[FromQuery] ConfirmOrder confirmation, 
														[FromQuery] Status status,
														[FromBody] TripRequest request)
		{
			try
			{
				if (confirmation == ConfirmOrder.Accept)
				{
					await _driverService.UpdateStatus(driverId, request.CustomerId, request.PaymentId, request.Status, request.PickUpAddress,
																request.PickUpWard, request.DropOffAddress, request.DropOffWard, status);

				}
				else if (confirmation == ConfirmOrder.Decline)
				{

				}
			} catch (Exception e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
			return Ok("Update booking status successfully");
		}*/

		/// <summary>
		/// Accept the trip
		/// </summary>
		/// <param name="request">Trip information</param>
		[HttpPost("accept")]
		public async Task<IActionResult> AcceptRequest([FromBody] TripResponse request)
		{
			await _driverService.UpdateStatus2(long.Parse(request.DriverId), Status.INPROGRESS);
			var updateResult = await _tripService.UpdateTrip(long.Parse(request.Id), long.Parse(request.DriverId), OrderStatus.INPROGRESS.ToString());
			var trip = await _tripService.GetTripById(long.Parse(request.Id));
			if (updateResult)
			{
				var response = new ApiResponseModel<TripResponse>
				{
					StatusCode = HttpStatusCode.OK,
					Message = "Driver accepted this trip.",
					Data = _mapper.Map<TripResponse>(trip),
				};
				return Ok(response);
			}
			else
			{
				var errorResponse = new ApiResponseModel<string>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Failed to accept the trip.",
					Data = null
				};
				return BadRequest(errorResponse);
			}
		}

		/// <summary>
		/// Reject the trip
		/// </summary>
		/// <param name="request">Trip information</param>
		[HttpPost("reject")]
		public async Task<IActionResult> RejectRequest([FromBody] TripResponse request)
		{
			await _driverService.UpdateStatus2(long.Parse(request.DriverId), Status.TEMPORARILY_LOCKED);
			var trip = await _tripService.GetTripById(long.Parse(request.Id));

			var response = new ApiResponseModel<TripResponse>
			{
				StatusCode = HttpStatusCode.OK,
				Message = "Driver rejected this trip.",
				Data = _mapper.Map<TripResponse>(trip),
			};
			return Ok(response);
		}

		/// <summary>
		/// Confirm order successful
		/// </summary>
		/// <param name="request">Trip information</param>
		[HttpPost("done-order")]
		public async Task<IActionResult> UpdateTripStatus([FromBody] TripResponse request)
		{
			var trip = await _tripService.GetTripById(long.Parse(request.Id));
			if(trip == null)
			{
				var tripResponse = new ApiResponseModel<TripResponse>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "Not found this trip.",
					Data = _mapper.Map<TripResponse>(trip),
				};
				return NotFound(tripResponse);
			}
			await _driverService.UpdateStatus2(driverId: long.Parse(request.DriverId), status2: Status.FREE);
			bool updatedOrder = await _tripService.UpdateTrip(long.Parse(request.Id), long.Parse(request.DriverId), OrderStatus.COMPLETED.ToString());
			trip = await _tripService.GetTripById(long.Parse(request.Id));

			if (updatedOrder)
			{
				var tripResponse = new ApiResponseModel<TripResponse>
				{
					StatusCode = HttpStatusCode.OK,
					Message = "Order completed.",
					Data = _mapper.Map<TripResponse>(trip),
				};
				return Ok(tripResponse);
			} else
			{
				var errorResponse = new ApiResponseModel<string>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Failed to update the trip.",
					Data = null
				};
				return BadRequest(errorResponse);
			}
		}
	}
}
