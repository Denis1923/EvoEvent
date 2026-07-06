using EvoEvent.Users.Domain.Enums;

namespace EvoEvent.Users.Application.DTOs
{
	public class UserDto
	{
		public Guid UserId { get; set; }

		public string Login { get; set; }

		public string Password { get; set; }

		public Roles Role { get; set; }
	}
}
