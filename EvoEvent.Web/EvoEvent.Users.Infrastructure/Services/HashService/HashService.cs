using System.Security.Cryptography;
using System.Text;

namespace EvoEvent.Users.Infrastructure.Services
{
	public class HashService : IHashService
	{
		public string ConvertHashPassword(string password)
		{
			var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
			return Convert.ToHexString(bytes);
		}

		public bool VerifyHashPassword(string inPassword, string hashPassword)
		{
			var hashInPassword = ConvertHashPassword(inPassword);

			return hashInPassword.Equals(hashPassword, StringComparison.Ordinal);
		}
	}
}
