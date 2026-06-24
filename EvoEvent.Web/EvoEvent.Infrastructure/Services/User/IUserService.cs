using EvoEvent.Application.DTOs;
using EvoEvent.Domain.Entities;

namespace EvoEvent.Infrastructure.Services
{
	public interface IUserService
	{
		Task RegisterUserAsync(UserDto userDto, CancellationToken token = default);

		Task<string> LoginUserAsync(UserDto userDto, CancellationToken token = default);
	}
}
