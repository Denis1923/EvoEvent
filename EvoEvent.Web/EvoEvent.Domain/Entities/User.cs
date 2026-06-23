using EvoEvent.Domain.Enums;

namespace EvoEvent.Domain.Entities
{
	public class User
	{
		public Guid Id { get; init; }

		public string Login { get; init; }

		public string HashPassword { get; private set; }

		public Roles Role { get; set; }

		public IEnumerable<Booking> Bookings { get; set; }

		public User()
		{
			
		}

		public User(Guid userId, string login, string password, Roles role)
		{
			Id = userId;
			Login = login;
			HashPassword = password;
			Role = role;
		}
	}
}
