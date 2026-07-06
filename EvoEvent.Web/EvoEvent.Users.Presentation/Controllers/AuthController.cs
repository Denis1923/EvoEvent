using EvoEvent.Users.Application.DTOs;
using EvoEvent.Users.Infrastructure.Services;
using EvoEvent.Users.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoEvent.Users.Presentation.Controllers
{
	[Route("auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IUserService _userService;

		public AuthController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequestDto registerRequest, CancellationToken token)
		{
			var userDto = new UserDto
			{
				UserId = Guid.NewGuid(),
				Login = registerRequest.Login,
				Password = registerRequest.Password,
				Role = registerRequest.Role.Value
			};

			await _userService.RegisterUserAsync(userDto, token);

			return NoContent();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequestDto loginRequest, CancellationToken token)
		{
			var userDto = new UserDto
			{
				Login = loginRequest.Login,
				Password = loginRequest.Password
			};

			var jwtToken = await _userService.LoginUserAsync(userDto, token);

			var response = new LoginResponseDto
			{
				Token = jwtToken
			};

			return Ok(response);
		}
	}
}
