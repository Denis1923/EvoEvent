using EvoEvent.Users.Domain.Enums;

namespace EvoEvent.Users.Domain.Entities
{
	public class User
	{
		public Guid UserId { get; init; }

		public string Login { get; init; }

		public string HashPassword { get; private set; }

		public Roles Role { get; set; }

		//public IEnumerable<Booking> Bookings { get; set; }

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
