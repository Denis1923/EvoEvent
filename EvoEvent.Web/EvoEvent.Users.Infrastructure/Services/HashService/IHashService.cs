using System;
using System.Collections.Generic;
using System.Text;

namespace EvoEvent.Users.Infrastructure.Services
{
	public interface IHashService
	{
		string ConvertHashPassword(string password);

		bool VerifyHashPassword(string inPassword, string hashPassword);
	}
}
