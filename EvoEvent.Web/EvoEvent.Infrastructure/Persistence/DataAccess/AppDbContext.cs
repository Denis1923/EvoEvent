using EvoEvent.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvoEvent.Infrastructure.Persistence.DataAccess
{
	public sealed class AppDbContext : DbContext
	{
		public DbSet<Event> Events => Set<Event>();
		public DbSet<Booking> Bookings => Set<Booking>();
		public DbSet<User> Users => Set<User>();

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		}
	}
}
