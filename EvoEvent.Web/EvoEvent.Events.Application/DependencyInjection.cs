using EvoEvent.Events.Application.Services;
using EvoEvent.Events.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EvoEvent.Events.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped<IEventService, EventService>();

			return services;
		}
	}
}
