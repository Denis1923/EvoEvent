using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Presentation.Models
{
	public class LoginRequestDto
	{
		[Required(ErrorMessage = "Заполните Логин (\"Login\": \"\")")]
		public string Login { get; set; }

		[Required(ErrorMessage = "Заполните Пароль (\"Password\": \"\")")]
		public string Password { get; set; }
	}
}