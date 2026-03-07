using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Models;

public class EventDto
{
	[Required]
	public string Title { get; set; }

	public string Description { get; set; }

	[Required]
	public DateTime StartAt { get; set; }

	[Required]
	public DateTime EndAt { get; set; }
}
