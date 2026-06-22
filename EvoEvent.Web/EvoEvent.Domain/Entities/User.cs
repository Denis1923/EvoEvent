using EvoEvent.Domain.Enums;
using System.Data;

namespace EvoEvent.Domain.Entities
{
	public class User
	{
		public Guid UserId { get; private set; }

		public string Login { get; private set; }

		public string HashPassword { get; private set; }

		public Roles Role { get; set; }

		public IEnumerable<Booking> Bookings { get; set; }

		public User()
		{
			
		}

		public User(Guid userId, string login, string password, Roles role)
		{
			UserId = userId;
			Login = login;
			HashPassword = password;
			Role = role;
		}
	}
}
