using EvoEvent.Domain.Entities;

namespace EvoEvent.Web.Models;

/// <summary>
/// Входная модель сущности "Событие"
/// </summary>
public class EventDto
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string? Description { get; set; }
	public DateTime StartAt { get; set; }
	public DateTime EndAt { get; set; }
	public int TotalSeats { get; set; }
	public int AvailableSeats { get; set; }
	public IEnumerable<Booking> Bookings { get; set; }
}
