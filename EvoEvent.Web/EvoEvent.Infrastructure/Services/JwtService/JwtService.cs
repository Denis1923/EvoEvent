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
			var claims = new Dictionary<string, object>
			{
				[JwtRegisteredClaimNames.Sub] = user.UserId.ToString(),
				["role"] = user.Role.ToString(),
				["login"] = user.Login,
				[JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
			};

			var expiresStr = configuration["Jwt:Expires"];
			var expiresMin = 15;

			if (int.TryParse(expiresStr, out int expiresMinOut)) 
				expiresMin = expiresMinOut;

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var descriptor = new SecurityTokenDescriptor
			{
				Issuer = configuration["Jwt:Issuer"],
				Audience = configuration["Jwt:Audience"],
				Claims = claims,
				NotBefore = DateTime.UtcNow,
				Expires = DateTime.Now.AddMinutes(expiresMin),
				IssuedAt = DateTime.UtcNow,
				SigningCredentials = creds
			};

			var tokenString = new JsonWebTokenHandler().CreateToken(descriptor);

			return tokenString;
		}
	}
}
