using EvoEvent.Users.Application.Abstractions.Repositories;
using EvoEvent.Users.Infrastructure.Persistence.DataAccess;
using EvoEvent.Users.Infrastructure.Services;
using EvoEvent.Users.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvoEvent.Users.Infrastructure
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

			services.AddScoped<IJwtService, JwtService>();
			services.AddScoped<IHashService, HashService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IUserRepository, UserRepository>();

			return services;
		}
	}
}
