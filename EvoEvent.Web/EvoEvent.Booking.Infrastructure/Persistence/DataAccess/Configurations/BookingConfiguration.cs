using BookingEnt = EvoEvent.Booking.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoEvent.Booking.Infrastructure.Persistence.DataAccess.Configurations
{
	public class BookingConfiguration : IEntityTypeConfiguration<BookingEnt>
	{
		public void Configure(EntityTypeBuilder<BookingEnt> builder)
		{

			builder.ToTable("bookings", "catalog");

			builder.HasKey(b => b.Id);
			builder.Property(b => b.Id)
				.ValueGeneratedNever();

			builder.Property(b => b.Status)
				.HasConversion<string>();

			builder.HasOne(b => b.Event)
				.WithMany(e => e.Bookings)
				.HasForeignKey(b => b.EventId);

			builder.HasOne(b => b.User)
				.WithMany(u => u.Bookings)
				.HasForeignKey(b => b.UserId);
		}
	}
}
