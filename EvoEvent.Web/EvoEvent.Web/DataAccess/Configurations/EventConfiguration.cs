using EvoEvent.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoEvent.Web.DataAccess.Configurations
{
	public class EventConfiguration : IEntityTypeConfiguration<Event>
	{
		public void Configure(EntityTypeBuilder<Event> builder)
		{
			builder.ToTable("events", "catalog");

			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id)
				.ValueGeneratedNever();

			builder.Property(e => e.Title)
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(e => e.Description)
				.HasMaxLength(255);

			builder.Property(e => e.StartAt)
				.IsRequired();

			builder.Property(e => e.EndAt)
				.IsRequired();

			builder.Property(e => e.TotalSeats)
				.IsRequired();

			builder.HasMany(e => e.Bookings)
				.WithOne(b => b.Event)
				.HasForeignKey(b => b.EventId);
		}
	}
}
