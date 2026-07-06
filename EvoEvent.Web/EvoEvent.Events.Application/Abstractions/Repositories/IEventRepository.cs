using EvoEvent.Events.Domain.Entities;

namespace EvoEvent.Events.Application.Abstractions.Repositories
{
	public interface IEventRepository
	{
		Task AddEventAsync(Event newEvt, CancellationToken token = default);
		Task<Event?> GetEventByIdAsync(Guid id, CancellationToken token = default);
		Task<List<Event>> GetEventsAsync();
		void RemoveEvent(Event expEvt);
		Task SaveChangesAsync(CancellationToken token = default);
	}
}
