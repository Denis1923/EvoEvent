using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Services
{
	public class EventService : IEventService
	{
		private readonly IEventRepository _eventRepository;

		public EventService(IEventRepository eventRepository)
		{
			_eventRepository = eventRepository;
		}

		public async Task<List<Event>> GetAllAsync()
		{
			var events = await _eventRepository.GetEventsAsync();
			if (!events.Any())
				throw new NotFoundException($"Событий нет");

			return events;
		}

		public List<Event> GetEventsAboutWhen(
			List<Event> events,
			string? title = null,
			DateTime? from = null,
			DateTime? to = null)
		{
			if (!string.IsNullOrEmpty(title))
				events = events.Where(evt => evt.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase)).ToList();

			if (from.HasValue)
				events = events.Where(evt => from.Value.Date <= evt.StartAt.Date).ToList();

			if (to.HasValue)
				events = events.Where(evt => to.Value.Date >= evt.EndAt.Date).ToList();

			if (!events.Any())
				throw new NotFoundException($"Событий нет");

			return events;
		}

		public List<Event> GetEventsAboutPaginated(
			List<Event> events,
			int page = 1,
			int pageSize = 10)
		{
			return events
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToList();
		}

		public async Task<Event?> GetByIdAsync(Guid id, CancellationToken token = default)
		{
			var expEvt = await _eventRepository.GetEventByIdAsync(id, token);

			if (expEvt is null)
				throw new NotFoundException($"Не найдено событие с таким ИД {id}");

			return expEvt;
		}

		public async Task<Guid> AddEventAsync(Event newEvt, CancellationToken token = default)
		{
			if (newEvt.StartAt >= newEvt.EndAt)
				throw new ValidationException("Дата окончания должна быть позже Даты начала");

			await _eventRepository.AddEventAsync(newEvt, token);
			await _eventRepository.SaveChangesAsync(token);

			return newEvt.Id;
		}

		public void UpdateEvent(Event extEvt, Event updEvt)
		{
			extEvt.Update(updEvt);
		}

		public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
		{
			var expEvt = await _eventRepository.GetEventByIdAsync(id, token);

			if (expEvt is null)
				throw new NotFoundException($"Не найдено событие с таким ИД {id}");

			try
			{
				_eventRepository.RemoveEvent(expEvt);
				await _eventRepository.SaveChangesAsync(token);
			}
			catch (Exception ex) 
			{
				throw new Exception($"Не удалось удалить События.Ид:{id}, по причине:{ex.Message}");
			}

			return true;
		}
	}
}
