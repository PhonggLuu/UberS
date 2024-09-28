using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces.Services;
using UberSystem.Dto.Requests;
using UberSystem.Service;
using UberSytem.Dto.Requests;

namespace Api.Authentication.Controllers
{
	[Route("api/authentication")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly TokenService _tokenService;
		private readonly IEmailService _emailService;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public AuthController(IUserService userService, TokenService tokenService, IEmailService emailService, IMapper mapper , IConfiguration configuration)
		{
			_userService = userService;
			_tokenService = tokenService;
			_emailService = emailService;
			_mapper = mapper;
			_configuration = configuration;
		}

		/// <summary>
		/// Đăng ký tài khoản sử dụng mới.
		/// </summary>
		/// <returns></returns>
		[HttpPost("sign-up")]
		public async Task<IActionResult> SignUp([FromBody] SignupModel request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest("Invalid registration details.");
			}

			var existingUser = await _userService.FindById(request.Id);
			if (existingUser != null)
			{
				return Conflict("Id already exists.");
			}

			//Kiểm tra xem email đã tồn tại chưa
			existingUser = await _userService.FindByEmail(request.Email);
			if (existingUser != null)
			{
				return Conflict("Email already exists.");
			}

			var user = _mapper.Map<User>(request);

			/*var user = new User
			{
				Id = request.Id,
				Email = request.Email,
				UserName = request.UserName,
				Password = request.Password,
				Role = (int)request.Role,
				EmailVerified = false
			};*/

			var roles = new List<string> { user.Role.ToString() };
			var token = _tokenService.GenerateJwtToken(user, roles);

			user.EmailVerificationToken = token;

			await _userService.Add(user);

			// Tạo link xác thực
			//var verificationLink = $"https://localhost:7012/api/authentication/verify?token={token}";
			// Gửi email xác thực
			//await _emailService.SendVerificationEmailAsync(user.Email, verificationLink);
			await _emailService.SendVerificationEmailAsync(user.Email, token);

			return Ok("Check mail to verify your account.");
		}
		/// <summary>
		/// Xác thực người dùng bằng token
		/// </summary>
		/// <returns>Vai trò của người dùng.</returns>
		[HttpPost("verify")]
		public async Task<IActionResult> VerifyEmail(string token)
		{
			var user = await _userService.GetByVerificationToken(token);
			if (user == null)
			{
				return BadRequest("Invalid token or user not found.");
			}

			user.EmailVerified = true;
			await _userService.Update(user);

			return Ok("Verify successful.");
		}
		/// <summary>
		/// Đăng nhập vào hệ thống
		/// </summary>
		/// <returns>Trạng thái đăng nhập</returns>
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

			return Ok(user.Id);
		}

		/// <summary>
		/// Cập nhật lại thông tin người dùng
		/// </summary>
		/// <returns></returns>
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update([FromRoute] long id,
												[FromBody] UpdateModel request)
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
	}
}
