using EvoEvent.Application.Abstractions.Repositories;
using EvoEvent.Application.DTOs.User;
using EvoEvent.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Infrastructure.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IHashService _hashService;
		private readonly IJwtService _jwtService;
		private readonly IConfiguration _configuration;

		public UserService(
			IUserRepository userRepository,
			IHashService hashService,
			IJwtService jwtService,
			IConfiguration configuration
			)
		{
			_userRepository	= userRepository;
			_hashService = hashService;
			_jwtService = jwtService;
			_configuration = configuration;
		}

		public async Task<string> LoginUserAsync(UserDto userDto, CancellationToken token = default)
		{
			var userExp = await _userRepository.GetUserByLoginAsync(userDto.Login, token);

			if (userExp is null)
				throw new ValidationException($"Пользователя с таким логином {userDto.Login} нет в системе");

			var verifyUser = _hashService.VerifyHashPassword(userDto.Password, userExp.HashPassword);
			
			if (!verifyUser)
				throw new ValidationException("Введен не верный пароль");

			return _jwtService.GeneratJwtTokena(userExp, _configuration);
		}

		public async Task RegisterUserAsync(UserDto userDto, CancellationToken token = default)
		{
			var user = new User(
				Guid.NewGuid(),
				userDto.Login,
				userDto.Password,
				userDto.Role
				);

			await _userRepository.CreateUserAsync(user, token);
			await _userRepository.SaveChangesAsync(token);
		}
	}
}
