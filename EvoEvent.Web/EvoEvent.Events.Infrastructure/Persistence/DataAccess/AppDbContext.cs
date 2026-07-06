using EvoEvent.Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvoEvent.Events.Infrastructure.Persistence.DataAccess
{
	public sealed class AppDbContext : DbContext
	{
		public DbSet<Event> Events => Set<Event>();

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		}
	}
}
