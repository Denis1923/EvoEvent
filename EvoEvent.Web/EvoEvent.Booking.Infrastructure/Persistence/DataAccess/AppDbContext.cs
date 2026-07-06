using BookingEnt = EvoEvent.Booking.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;

namespace EvoEvent.Booking.Infrastructure.Persistence.DataAccess
{
	public sealed class AppDbContext : DbContext
	{
		public DbSet<BookingEnt> Bookings => Set<BookingEnt>();

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		}
	}
}
