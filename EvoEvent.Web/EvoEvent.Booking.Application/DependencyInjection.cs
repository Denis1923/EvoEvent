using EvoEvent.Booking.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EvoEvent.Booking.Application;
public static class DependencyInjection
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddScoped<IBookingService, BookingService>();

		return services;
	}
}
