using EvoEvent.Booking.Application.Abstractions;
using EvoEvent.Booking.Infrastructure.Persistence.DataAccess;
using EvoEvent.Booking.Infrastructure.Services;
using EvoEvent.Booking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EvoEvent.Booking.Application.Abstractions.Repositories;

namespace EvoEvent.Booking.Booking.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionStr = configuration.GetConnectionString("DefaultConnection");

			services.AddDbContext<AppDbContext>(options =>
			{
				options.UseNpgsql(connectionStr);
			});

			services.AddScoped<IBookingRepository, BookingRepository>();
			services.AddHostedService<BookingBackgroundService>();

			return services;
		}
	}
}
