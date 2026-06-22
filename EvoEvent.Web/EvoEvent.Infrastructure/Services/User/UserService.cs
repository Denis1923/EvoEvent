using EvoEvent.Application.Abstractions.Repositories;
using EvoEvent.Domain.Entities;
using Microsoft.Extensions.Configuration;

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

		public async Task<string> LoginUserAsync(User user, CancellationToken token = default)
		{
			var userExp = await _userRepository.GetUserByLoginAsync(user.Login, token);

			if (userExp is null)
				throw new Exception("todo: заменить exc");

			var verifyUser = _hashService.VerifyHashPassword(user.HashPassword, userExp.HashPassword);
			
			if (!verifyUser)
				throw new Exception("todo: заменить exc");

			return _jwtService.GeneratJwtTokena(user, _configuration);
		}

		public async Task RegisterUserAsync(User user, CancellationToken token = default)
		{
			await _userRepository.CreateUserAsync(user, token);
			await _userRepository.SaveChangesAsync(token);
		}
	}
}
