using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services
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

		Task<Guid> AddEventAsync(Event evt, CancellationToken token = default);

		void UpdateEvent(Event extUpd, Event updEvt);

		Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
	}
}
