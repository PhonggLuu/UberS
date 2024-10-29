using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Dto.Requests;
using UberSystem.Service;
using UberSytem.Dto.Requests;
using UberSytem.Dto;
using Microsoft.AspNetCore.OData.Formatter;
using UberSystem.Common.Enums;

namespace Api.Authentication.Controllers
{
	[Route("odata/authentication")]
	[ApiController]
	public class AuthController : ODataController
	{
		private readonly IUserService _userService;
		private readonly TokenService _tokenService;
		private readonly IEmailService _emailService;
		private readonly IMapper _mapper;

		public AuthController(IUserService userService, TokenService tokenService, IEmailService emailService, IMapper mapper)
		{
			_userService = userService;
			_tokenService = tokenService;
			_emailService = emailService;
			_mapper = mapper;
		}

		/// <summary>
		/// Registers a new account using the provided email.
		/// </summary>
		/// <param name="request">The registration request containing user details.</param>
		/// <returns>A task representing the asynchronous operation, with a result indicating success or failure.</returns>
		[HttpPost("sign-up")]
		public async Task<IActionResult> SignUp([FromBody] SignupModel request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest("Invalid registration details.");
			}

			var existingUser = await _userService.FindByEmail(request.Email);
			if (existingUser != null)
			{
				return Conflict("Email already exists.");
			}

			bool isValid = Enum.IsDefined(typeof(UserRole), request.Role);
			if (!isValid)
			{
				return BadRequest("Invalid role.");
			}

			var user = _mapper.Map<User>(request);
			user.Id = Helper.GenerateRandomLong();
			var roles = new List<string> { request.Role.ToString() };
			var token = _tokenService.GenerateJwtToken(user, roles);

			user.EmailVerificationToken = token;

			await _userService.Add(user);
			await _emailService.SendVerificationEmailAsync(user.Email, token);

			return Ok("Check mail to verify your account.");
		}

		/// <summary>
		/// Verifies the user's email address using the provided verification token.
		/// </summary>
		/// <param name="token">The email verification token.</param>
		/// <returns>A task representing the asynchronous operation, indicating the result of the verification.</returns>
		[HttpPost("verify")]
		public async Task<IActionResult> VerifyEmail(string token)
		{
			try
			{
				var user = await _userService.GetByVerificationToken(token);
				if (user == null)
				{
					return BadRequest("Invalid token or user not found.");
				}
				user.EmailVerified = true;
				await _userService.Update(user);
			}
			catch (Exception e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}

			return Ok("Verify successful.");
		}

		/// <summary>
		/// Signs in the user by validating their email and password.
		/// </summary>
		/// <param name="request">The login request containing email and password.</param>
		/// <returns>A task representing the asynchronous operation, with a result containing the user's ID, role, and authentication token.</returns>
		[HttpPost("sign-in")]
		public async Task<IActionResult> SignIn([FromBody] LoginModel request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest("Invalid login details.");
			}

			var user = await _userService.FindByEmail(request.Email);
			if (user == null)
			{
				return NotFound("Email not found.");
			}

			if (user.Password != request.Password)
			{
				return Unauthorized("Invalid password.");
			}

			if (!user.EmailVerified)
			{
				return Unauthorized("Email not verified.");
			}

			var token = _tokenService.GenerateJwtToken(user, new List<string> { user.Role.ToString() });
			user.EmailVerificationToken = token;
			await _userService.Update(user);

			return Ok(new { userId = user.Id, role = user.Role.ToString(), token });
		}

		/// <summary>
		/// Updates the user's information based on the provided user ID and update model.
		/// </summary>
		/// <param name="id">The ID of the user to be updated.</param>
		/// <param name="request">The update model containing the new user information.</param>
		/// <returns>A task representing the asynchronous operation, indicating success or failure of the update.</returns>
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update([FromODataUri] long id, [FromBody] UpdateModel request)
		{
			var user = await _userService.FindById(id);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			_mapper.Map(request, user);
			await _userService.Update(user);

			return Ok("Update successful.");
		}

		/// <summary>
		/// Deletes a user account based on the provided user ID.
		/// </summary>
		/// <param name="id">The ID of the user to be deleted.</param>
		/// <returns>A task representing the asynchronous operation, indicating success or failure of the deletion.</returns>
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete([FromODataUri] long id)
		{
			var user = await _userService.FindById(id);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			await _userService.Delete(user);

			return Ok("User deleted successfully.");
		}

	}
}
