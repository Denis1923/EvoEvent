using EvoEvent.Application.Abstractions.Repositories;
using EvoEvent.Domain.Entities;
using EvoEvent.Infrastructure.Persistence.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EvoEvent.Infrastructure.Persistence.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly AppDbContext _context;

		public UserRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task CreateUserAsync(User user, CancellationToken token = default)
			=> await _context.Users.AddAsync(user, token);

		public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken token = default)
			=> await _context.Users.FirstOrDefaultAsync(u => u.UserId == id, token);

		public async Task<User?> GetUserByLoginAsync(string login, CancellationToken token = default)
			=> await _context.Users.FirstOrDefaultAsync(u => u.Login == login, token);

		public async Task SaveChangesAsync(CancellationToken token = default)
			=> await _context.SaveChangesAsync(token);
	}
}
