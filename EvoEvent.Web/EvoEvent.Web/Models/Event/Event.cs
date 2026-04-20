namespace EvoEvent.Web.Models;

public class Event
{
	public Guid Id { get; init; }
	public string Title { get; private set; }
	public string? Description { get; private set; }
	public DateTime StartAt { get; private set; }
	public DateTime EndAt { get; private set; }
	public int TotalSeats { get; init; }
	public int AvailableSeats { get; set; }

	public Event() { }
	public Event(
		Guid? id, 
		string title, 
		string? description, 
		DateTime startAt, 
		DateTime endAt,
		int totalSeats,
		int? availableSeats = null)
	{
		Id = id ?? Guid.NewGuid();
		Title = title;
		Description = description;
		StartAt = startAt;
		EndAt = endAt;	
		TotalSeats = totalSeats;
		AvailableSeats = availableSeats ?? TotalSeats;
	}

	public void Update(Event updEvent)
	{
		Title = updEvent.Title;
		Description = updEvent.Description;
		StartAt = updEvent.StartAt;
		EndAt = updEvent.EndAt;
	}

	public bool TryReserveSeats(int count = 1)
	{
		if (AvailableSeats == 0)
			return false;

		var checkAvailableSeats = (AvailableSeats - count) < 0;
		if (checkAvailableSeats)
			return false;

		AvailableSeats -= count;
		return true;
	}

	public void ReleaseSeats(int count = 1)
	{
		var checkAvailableSeats = (AvailableSeats + count) <= AvailableSeats;

		if (checkAvailableSeats
			&& AvailableSeats < TotalSeats)
			AvailableSeats += count;
	}
}
