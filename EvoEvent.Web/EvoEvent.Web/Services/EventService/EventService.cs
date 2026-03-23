using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EvoEvent.Web.Services
{
	public class EventService : IEventService
	{
		private static readonly List<Event> _events = new();

		public EventService()
		{
			
		}

		public IEnumerable<Event> GetAll()
		{
			if (!_events.Any())
				throw new NotFoundException($"Событий нет");

			return _events;
		}

		public IEnumerable<Event> GetEventsAboutWhen(
			IEnumerable<Event> events,
			string? title = null,
			DateTime? from = null,
			DateTime? to = null)
		{
			if (!string.IsNullOrEmpty(title))
				events = events.Where(evt => evt.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase));

			if (from.HasValue)
				events = events.Where(evt => from.Value.Date <= evt.StartAt.Date);

			if (to.HasValue)
				events = events.Where(evt => to.Value.Date >= evt.EndAt.Date);

			if (!events.Any())
				throw new NotFoundException($"Событий нет");

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
		{
			var extEvt = _events.FirstOrDefault(e => e.Id == id);

			if (extEvt is null)
				throw new NotFoundException($"Не найдено событие с таким ИД {id}");

			return extEvt;
		}

		public Guid AddEvent(Event newEvt)
		{
			if (newEvt.StartAt >= newEvt.EndAt)
				throw new ValidationException("Дата окончания должна быть позже Даты начала");

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
				throw new NotFoundException($"Не найдено событие с таким ИД {id}");

			return _events.Remove(extEvt);
		}
	}
}
