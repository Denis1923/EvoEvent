using EvoEvent.Web.Validation;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Models;

/// <summary>
/// Входная модель сущности "Событие"
/// </summary>
public class EventDto
{
	/// <summary>
	/// Наименование события
	/// </summary>
	[Required(ErrorMessage = "Заполните наименования события (\"Title\": \"\")")]
	public string Title { get; set; }

	/// <summary>
	/// Описание события
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Стартовая дата события 
	/// </summary>
	[Required(ErrorMessage = "Заполните стартовую дату события (\"StartAt\": \"YYYY-mm-DD\")")]
	public DateTime StartAt { get; set; }

	/// <summary>
	/// Дата окончания события
	/// </summary>
	[Required(ErrorMessage = "Заполните дату окончания события (\"EndAt\": \"YYYY-mm-DD\")")]
	[EndDateLaterThanStart]
	public DateTime EndAt { get; set; }
}
