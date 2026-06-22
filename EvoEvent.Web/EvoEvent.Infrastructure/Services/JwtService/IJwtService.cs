using EvoEvent.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoEvent.Infrastructure.Services
{
	public interface IJwtService
	{
		string GeneratJwtTokena(User user, IConfiguration configuration);
	}
}
