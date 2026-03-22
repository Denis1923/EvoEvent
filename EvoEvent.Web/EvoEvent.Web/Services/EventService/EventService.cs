using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services
{
	public class EventService : IEventService
	{
		private static readonly List<Event> _events = new();

		public EventService()
		{
			
		}

		public IEnumerable<Event> GetAll() 
			=> _events;

		public IEnumerable<Event> GetEventsAboutWhen(
			IEnumerable<Event> events,
			string? title = null,
			DateTime? from = null,
			DateTime? to = null)
		{
			if (!string.IsNullOrEmpty(title))
				events = events.Where(evt => evt.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase));

			if (from.HasValue)
				events = events.Where(evt => from <= evt.StartAt);

			if (to.HasValue)
				events = events.Where(evt => to >= evt.EndAt);

			return events;
		}

		public IEnumerable<Event> GetEventsAboutPaginated(
			IEnumerable<Event> events,
			int page = 1,
			int pageSize = 10)
		{
			return events
					.Skip((page - 1) * pageSize)
					.Take(pageSize);
		}

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
