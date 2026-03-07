using static System.Net.WebRequestMethods;

namespace EvoEvent.Web.Models;

public class Event
{
	public Guid Id { get; private set; }

	private string _title;

	private string _description;

	private DateTime _startAt;

	private DateTime _endAt;

	public Event(string title, string description, DateTime startAt, DateTime endAt)
	{
		Id = Guid.NewGuid();
		_title = title;
		_description = description;
		_startAt = startAt;
		_endAt = endAt;		
	}

	public void Update(string title, string description, DateTime startAt, DateTime endAt)
	{
		_title = title;
		_description = description;
		_startAt = startAt;
		_endAt = endAt;
	}
}
