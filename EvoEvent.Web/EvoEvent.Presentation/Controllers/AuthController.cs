using EvoEvent.Application.DTOs.User;
using EvoEvent.Infrastructure.Services;
using EvoEvent.Presentation.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace EvoEvent.Presentation.Controllers
{
	[Route("[controller]")]
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
				Login = registerRequest.Login,
				Password = registerRequest.Password,
				Role = registerRequest.Role.Value
			};

			await _userService.RegisterUserAsync(userDto, token);

			return Ok();
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

			return Ok(new { Token = jwtToken });
		}
	}
}
