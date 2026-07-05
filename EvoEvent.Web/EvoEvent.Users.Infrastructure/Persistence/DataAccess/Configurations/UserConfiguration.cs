using EvoEvent.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoEvent.Users.Infrastructure.Persistence.DataAccess.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("users", "catalog");

			builder.HasKey(e => e.UserId);
			
			builder.Property(e => e.UserId)
				.ValueGeneratedNever();

			builder.HasIndex(e => e.Login)
				.IsUnique();

			//builder.HasMany(e => e.Bookings)
			//	.WithOne(b => b.User)
			//	.HasForeignKey(b => b.UserId);
		}
	}
}
