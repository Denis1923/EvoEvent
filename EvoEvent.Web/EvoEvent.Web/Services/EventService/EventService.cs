using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services
{
	public class EventService : IEventService
	{
		private List<Event> _events = new();

		public IEnumerable<Event> GetAll() 
			=> _events;

		public Event? GetById(Guid id)
			=> _events.FirstOrDefault(e => e.Id == id);

		public void AddEvent(Event newEvt) 
			=> _events.Add(newEvt);

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
			return _events.Remove(extEvt);
		}
	}
}
