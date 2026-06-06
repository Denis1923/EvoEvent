using EvoEvent.Application.Abstractions;
using EvoEvent.Infrastructure.Persistence.DataAccess;
using EvoEvent.Infrastructure.Services;
using EvoEvent.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvoEvent.Infrastructure
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

			services.AddScoped<IEventRepository, EventRepository>();
			services.AddScoped<IBookingRepository, BookingRepository>();

			services.AddHostedService<BookingBackgroundService>();

			return services;
		}
	}
}
