using EvoEvent.Application.Abstractions;
using EvoEvent.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoEvent.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped<IEventService, EventService>();
			services.AddScoped<IBookingService, BookingService>();

			return services;
		}
	}
}
