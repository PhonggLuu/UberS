using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using UberSystem.Common.Enums;
using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Dto.Requests;
using UberSystem.Dto.Responses;
using UberSytem.Dto;

namespace UberSystem.Api.Customer.Controllers
{

    [Authorize(Roles = "Customer")]
	[Route("odata/customer")]
	[ApiController]
	public class CustomerController : ODataController
	{
		private readonly IDriverService _driverService;
		private readonly ILocateService _locateService;
		private readonly IRatingService _ratingService;
		private readonly ITripService _tripService;
		private readonly INotificationService _notificationService;
		private readonly IMapper _mapper;

		public CustomerController(IDriverService driverService, ILocateService locateService, IRatingService ratingService, ITripService tripService, INotificationService notificationService, IMapper mapper)
		{
			_driverService = driverService;
			_locateService = locateService;
			_ratingService = ratingService;
			_tripService = tripService;
			_notificationService = notificationService;
			_mapper = mapper;
		}

		/// <summary>
		/// Retrieves the nearest driver with the highest rating.
		/// </summary>
		/// <returns>A task representing the asynchronous operation, containing the nearest driver with the best rating.</returns>
		[HttpPost("get-driver/{customerId}")]
		public async Task<IActionResult> GetNearestDrivers([FromRoute] long customerId, [FromBody] CustomerLocation request)
		{
			// Get the coordinates of the pick-up address
			var location = await _locateService.GetCoordinates(request.PickUpAddress, request.PickUpWard);
			var driver = await _driverService.GetDriversHighRating(location.StartLatitude, location.StartLongitude);
			if (driver == null)
			{
				return NotFound(new ApiResponseModel<TripResponse> { 
					StatusCode = System.Net.HttpStatusCode.NotFound, 
					Message = "No drivers available.", 
					Data = null
				});
			}
			//_mapper.Map<DriverResponse>(driver);
			/*var message = await _notificationService.SendNotificationToDrivers(driver.Id, request);
			return Ok(new { Customer = customerId, CustomerLocation = location, Driver = driver, Notification = message});*/
			var trip = await _tripService.GetCustomerTripPending(customerId);
			if (trip != null)
			{
				trip.DriverId = driver.Id;
				await _tripService.UpdateTrip(customerId, driver.Id, null);
			} else
			{
				trip = new Trip
				{
					Id = Helper.GenerateRandomLong(),
					CustomerId = customerId,
					DriverId = driver.Id,
					SourceLatitude = location.StartLatitude,
					SourceLongitude = location.StartLongitude,
					DestinationLatitude = location.EndLatitude,
					DestinationLongitude = location.EndLongitude,
					Status = OrderStatus.PENDING.ToString(),
					CreateAt = BitConverter.GetBytes(DateTime.Now.ToBinary()),
				};
				await _tripService.AddNewTrip(trip);
			}
			var tripRequest = _mapper.Map<TripResponse>(trip);
			var response = new ApiResponseModel<TripResponse>
			{
				StatusCode = System.Net.HttpStatusCode.OK,
				Message = "Waiting for driver accept booking.",
				Data = tripRequest,
			};
			return Ok(response);
			// Tạo trip với trạng thái đang chờ để tài xế có thể nhận đơn.
		}

		/// <summary>
		/// Submits a rating for a driver.
		/// </summary>
		/// <returns>A task representing the asynchronous operation, indicating the result of the rating submission.</returns>
		[HttpPost("rating")]
		public async Task<IActionResult> RateDriver([FromBody] RatingRequest request)
		{
			var trip = await _tripService.GetTripById(request.TripId);
			ApiResponseModel<RatingResponse>? response;
			if (trip == null)
			{
				response = new ApiResponseModel<RatingResponse>
				{
					StatusCode = System.Net.HttpStatusCode.NotFound,
					Message = "Not found this trip.",
					Data = null,
				};
				return NotFound(response);
			}
			await _ratingService.RateDriver(request.TripId, request.Rating1, request.Feedback);
			var rating = _ratingService.GetRatingAsync(request.TripId);
			response = new ApiResponseModel<RatingResponse>
			{
				StatusCode = System.Net.HttpStatusCode.OK,
				Message = "Rating driver successfully.",
				Data = _mapper.Map<RatingResponse>(rating),
			};
			return Ok(response);
		}
	}
}
