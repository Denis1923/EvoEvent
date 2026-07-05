using EvoEvent.Users.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace EvoEvent.Users.Infrastructure.Services
{
	public interface IJwtService
	{
		string GeneratJwtTokena(User user, IConfiguration configuration);
	}
}
