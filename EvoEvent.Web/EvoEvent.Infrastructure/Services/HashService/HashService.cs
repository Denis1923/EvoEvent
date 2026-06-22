using System.Security.Cryptography;
using System.Text;

namespace EvoEvent.Infrastructure.Services
{
	public class HashService : IHashService
	{
		public string ConvertHashPassword(string password)
		{
			var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
			return Convert.ToHexString(bytes);
		}
	}
}
