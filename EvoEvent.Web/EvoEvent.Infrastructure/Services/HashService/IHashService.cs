using System;
using System.Collections.Generic;
using System.Text;

namespace EvoEvent.Infrastructure.Services
{
	public interface IHashService
	{
		string ConvertHashPassword(string password);
	}
}
