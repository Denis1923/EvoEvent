using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services
{
	public class EventService : IEventService
	{
		private List<Event> _events = new();

		public IEnumerable<Event> GetAll() => _events;

		public Event? GetById(Guid id)
			=> _events.FirstOrDefault(e => e.Id == id);

		public void AddEvent(EventDto evt)
		{
			Event newEvent = new Event(
				evt.Title, 
				evt.Description, 
				evt.StartAt, 
				evt.EndAt);

			_events.Add(newEvent);
		}

		public void Save(Guid id, EventDto evt)
		{
			var extEvt = _events.FirstOrDefault(e => e.Id == id);
			extEvt.Update(
				evt.Title,
				evt.Description,
				evt.StartAt,
				evt.EndAt
				);
		}

		public bool DeleteById(Guid id)
		{
			var extEvt = _events.FirstOrDefault(e => e.Id == id);
			return _events.Remove(extEvt);
		}
	}
}
