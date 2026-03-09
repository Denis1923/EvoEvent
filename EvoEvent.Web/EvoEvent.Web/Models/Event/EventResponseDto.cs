using EvoEvent.Web.Validation;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Models;

/// <summary>
/// Выходная модель сущности "Событие"
/// </summary>
public class EventResponseDto
{
	public Guid Id { get; set; }

	/// <summary>
	/// Наименование события
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// Описание события
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Стартовая дата события 
	/// </summary>
	public DateTime StartAt { get; set; }

	/// <summary>
	/// Дата окончания события
	/// </summary>
	public DateTime EndAt { get; set; }
}
