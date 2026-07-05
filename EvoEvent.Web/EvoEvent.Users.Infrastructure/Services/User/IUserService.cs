using EvoEvent.Users.Application.DTOs;
using EvoEvent.Users.Domain.Entities;

namespace EvoEvent.Users.Infrastructure.Services
{
	public interface IUserService
	{
		Task RegisterUserAsync(UserDto userDto, CancellationToken token = default);

		Task<string> LoginUserAsync(UserDto userDto, CancellationToken token = default);
	}
}
