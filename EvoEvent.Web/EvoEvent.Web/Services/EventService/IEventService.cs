using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services
{
	public interface IEventService
	{
		IEnumerable<Event> GetAll();

		IEnumerable<Event> GetEventsAboutWhen(
			IEnumerable<Event> events, 
			string? title = null, 
			DateTime? from = null, 
			DateTime? to = null);

		IEnumerable<Event> GetEventsAboutPaginated(
			IEnumerable<Event> events,
			int page = 1,
			int pageSize = 10);

		Task<Event?> GetByIdAsync(Guid id, CancellationToken token = default);

		Task<Guid> AddEventAsync(Event evt, CancellationToken token = default);

		void Save(Event extUpd, Event updEvt);

		Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
	}
}
