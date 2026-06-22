using EvoEvent.Domain.Entities;

namespace EvoEvent.Infrastructure.Services
{
	public interface IUserService
	{
		Task RegisterUserAsync(User user, CancellationToken token = default);

		Task<string> LoginUserAsync(User user, CancellationToken token = default);
	}
}
