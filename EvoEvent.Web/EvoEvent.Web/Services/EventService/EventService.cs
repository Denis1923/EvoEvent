using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services
{
	public class EventService : IEventService
	{
		private static readonly List<Event> _events = new();

		public IEnumerable<Event> GetAll() 
			=> _events;

		public Event? GetById(Guid id)
			=> _events.FirstOrDefault(e => e.Id == id);

		public Guid AddEvent(Event newEvt)
		{
			_events.Add(newEvt);
			return newEvt.Id;
		}

		public void Save(Event extEvt, Event updEvt)
		{
			extEvt.Update(
				updEvt.Title,
				updEvt.Description,
				updEvt.StartAt,
				updEvt.EndAt
				);
		}

		public bool DeleteById(Guid id)
		{
			var extEvt = _events.FirstOrDefault(e => e.Id == id);

			if (extEvt is null)
				return false;

			return _events.Remove(extEvt);
		}
	}
}
