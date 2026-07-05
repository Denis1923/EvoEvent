using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Users.Presentation.Models
{
	public class LoginRequestDto
	{
		[Required(ErrorMessage = "Заполните Логин (\"Login\": \"\")")]
		public string Login { get; set; }

		[Required(ErrorMessage = "Заполните Пароль (\"Password\": \"\")")]
		public string Password { get; set; }
	}
}