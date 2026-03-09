namespace EvoEvent.Web.Models;

public class Event
{
	public Guid Id { get; private set; }
	public string Title { get; private set; }
	public string? Description { get; private set; }
	public DateTime StartAt { get; private set; }
	public DateTime EndAt { get; private set; }

	public Event(string title, string? description, DateTime startAt, DateTime endAt)
	{
		Id = Guid.NewGuid();
		Title = title;
		Description = description;
		StartAt = startAt;
		EndAt = endAt;		
	}

	public void Update(string title, string? description, DateTime startAt, DateTime endAt)
	{
		Title = title;
		Description = description;
		StartAt = startAt;
		EndAt = endAt;
	}
}
