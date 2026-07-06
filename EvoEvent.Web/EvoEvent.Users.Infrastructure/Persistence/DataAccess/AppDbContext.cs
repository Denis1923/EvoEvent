using EvoEvent.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvoEvent.Users.Infrastructure.Persistence.DataAccess
{
	public sealed class AppDbContext : DbContext
	{
		public DbSet<User> Users => Set<User>();

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		}
	}
}
