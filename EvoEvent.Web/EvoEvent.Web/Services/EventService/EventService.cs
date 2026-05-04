using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Services
{
	public class EventService : IEventService
	{
		private readonly AppDbContext _context;

		public EventService(AppDbContext context)
		{
			_context = context;
		}

		public IEnumerable<Event> GetAll()
		{
			var events = _context.Events;
			if (!events.Any())
				throw new NotFoundException($"Событий нет");

			return events;
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

		public async Task<Event?> GetByIdAsync(Guid id, CancellationToken token = default)
		{
			var extEvt = await _context.Events.FirstOrDefaultAsync(e => e.Id == id, token);

			if (extEvt is null)
				throw new NotFoundException($"Не найдено событие с таким ИД {id}");

			return extEvt;
		}

		public async Task<Guid> AddEventAsync(Event newEvt, CancellationToken token = default)
		{
			if (newEvt.StartAt >= newEvt.EndAt)
				throw new ValidationException("Дата окончания должна быть позже Даты начала");

			await _context.Events.AddAsync(newEvt, token);
			await _context.SaveChangesAsync(token);

			return newEvt.Id;
		}

		public void Save(Event extEvt, Event updEvt)
		{
			extEvt.Update(updEvt);
		}

		public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
		{
			var extEvt = await _context.Events.FirstOrDefaultAsync(e => e.Id == id, token);

			if (extEvt is null)
				throw new NotFoundException($"Не найдено событие с таким ИД {id}");

			try
			{
				_context.Events.Remove(extEvt);
				await _context.SaveChangesAsync(token);
			}
			catch (Exception ex) 
			{
				throw new Exception($"Не удалось удалить События.Ид:{id}, по причине:{ex.Message}");
			}

			return true;
		}
	}
}
