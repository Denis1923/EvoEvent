using EvoEvent.Users.Domain.Entities;

namespace EvoEvent.Users.Application.Abstractions.Repositories
{
	public interface IUserRepository
	{
		Task CreateUserAsync(User user, CancellationToken token = default);

		Task<User?> GetUserByIdAsync(Guid id, CancellationToken token = default);

		Task<User?> GetUserByLoginAsync(string login, CancellationToken token = default);

		Task SaveChangesAsync(CancellationToken token = default);
	}
}
