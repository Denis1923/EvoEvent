using EvoEvent.Application.DTOs;
using EvoEvent.Domain.Enums;

namespace EvoEvent.Web.Tests.Models
{
	internal class ModelUserServiceTest
	{
		public static List<UserDto> GetUsers()
			=> new List<UserDto>()
			{
				new UserDto
				{
					UserId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479"),
					Login = "User",
					Password = "1234",
					Role = Roles.User
				},
				new UserDto
				{
					UserId = Guid.Parse("417dc10b-58cc-5172-a567-0e02b2c3d489"),
					Login = "Admin",
					Password = "1234",
					Role = Roles.Admin
				},
			};
	}
}
