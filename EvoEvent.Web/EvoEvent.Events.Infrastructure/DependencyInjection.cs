using EvoEvent.Application.Abstractions;
using EvoEvent.Events.Infrastructure.Persistence.DataAccess;
using EvoEvent.Events.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvoEvent.Events.Infrastructure
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

			return services;
		}
	}
}
