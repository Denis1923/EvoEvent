using EvoEvent.Events.Domain.Entities;
using EvoEvent.Events.Application.DTOs;

namespace EvoEvent.Events.Services
{
	public interface IEventService
	{
		Task<List<Event>> GetAllAsync();

		List<Event> GetEventsAboutWhen(
			List<Event> events, 
			string? title = null, 
			DateTime? from = null, 
			DateTime? to = null);

		List<Event> GetEventsAboutPaginated(
			List<Event> events,
			int page = 1,
			int pageSize = 10);

		Task<Event?> GetByIdAsync(Guid id, CancellationToken token = default);

		Task<Guid> AddEventAsync(EventDto evt, CancellationToken token = default);

		Task UpdateEventAsync(Event extUpd, EventDto updEvt, CancellationToken token = default);

		Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
	}
}
