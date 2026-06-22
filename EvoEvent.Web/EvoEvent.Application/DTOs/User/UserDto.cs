using EvoEvent.Domain.Enums;

namespace EvoEvent.Application.DTOs.User
{
	public class UserDto
	{
		public Guid UserId { get; set; }

		public string Login { get; set; }

		public string Password { get; set; }

		public Roles Role { get; set; }
	}
}
