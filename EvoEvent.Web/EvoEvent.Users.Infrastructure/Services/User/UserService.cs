using EvoEvent.Users.Application.DTOs;
using EvoEvent.Users.Domain.Entities;
using Microsoft.Extensions.Configuration;
using EvoEvent.Users.Application.Abstractions.Repositories;
using CommonLibrary.Exceptions;

namespace EvoEvent.Users.Infrastructure.Services
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
				throw new NotFoundException($"Пользователя с таким логином {userDto.Login} нет в системе");

			var verifyUser = _hashService.VerifyHashPassword(userDto.Password, userExp.HashPassword);
			
			if (!verifyUser)
				throw new InvalidOperationException("Введен не верный пароль");

			return _jwtService.GeneratJwtTokena(userExp, _configuration);
		}

		public async Task RegisterUserAsync(UserDto userDto, CancellationToken token = default)
		{
			var hashPassword = _hashService.ConvertHashPassword(userDto.Password);

			var user = new User(
				userDto.UserId,
				userDto.Login,
				hashPassword,
				userDto.Role
				);

			await _userRepository.CreateUserAsync(user, token);
			await _userRepository.SaveChangesAsync(token);
		}
	}
}
