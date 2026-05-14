using EvoEvent.Web.Models;

namespace EvoEvent.Web.Repositories
{
	public interface IEventRepository
	{
		Task AddEventAsync(Event newEvt, CancellationToken token);
		Task<Event?> GetEventByIdAsync(Guid id, CancellationToken token);
		Task<List<Event>> GetEventsAsync();
		void RemoveEvent(Event expEvt);
		Task SaveChangesAsync(CancellationToken token);
	}
}
