using EvoEvent.Domain.Enums;

namespace EvoEvent.Domain.Entities
{
	public class User(Guid userId, string login, string password, Roles role)
	{
		public Guid UserId { get; private set; } = userId;

		public string Login { get; private set; } = login;

		public string HashPassword { get; private set; } = password;

		public Roles Role { get; set; } = role;

	}
}
