using EvoEvent.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoEvent.Infrastructure.Persistence.DataAccess.Configurations
{
	public class BookingConfiguration : IEntityTypeConfiguration<Booking>
	{
		public void Configure(EntityTypeBuilder<Booking> builder)
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
