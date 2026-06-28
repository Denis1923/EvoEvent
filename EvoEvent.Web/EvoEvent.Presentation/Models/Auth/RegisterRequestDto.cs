using EvoEvent.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Presentation.Models
{
	public class RegisterRequestDto
	{
		[Required(ErrorMessage = "Заполните Логин (\"Login\": \"\")")]
		public string Login { get; set; }

		[Required(ErrorMessage = "Заполните Пароль (\"Password\": \"\")")]
		public string Password { get; set; }

		public Roles? Role { get; set; } = Roles.User;
	}
}