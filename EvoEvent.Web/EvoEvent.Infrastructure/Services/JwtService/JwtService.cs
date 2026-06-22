using EvoEvent.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EvoEvent.Infrastructure.Services
{
	public class JwtService : IJwtService
	{
		public JwtService() { }

		public string GeneratJwtTokena(User user, IConfiguration configuration)
		{
			// 1. Claims
			var claims = new Dictionary<string, object>
			{
				[JwtRegisteredClaimNames.Sub] = user.UserId.ToString(),
				["role"] = user.Role,
				["login"] = user.Login,
				[JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
			};

			// 2. Ключ и алгоритм подписи
			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// 3. Описание токена
			var descriptor = new SecurityTokenDescriptor
			{
				Issuer = configuration["Jwt:Issuer"],
				Audience = configuration["Jwt:Audience"],
				Claims = claims,
				NotBefore = DateTime.UtcNow,
				Expires = DateTime.UtcNow.AddMinutes(30),
				IssuedAt = DateTime.UtcNow,
				SigningCredentials = creds
			};

			// 4. Генерация строки токена
			var tokenString = new JsonWebTokenHandler().CreateToken(descriptor);

			return tokenString;
		}
	}
}
