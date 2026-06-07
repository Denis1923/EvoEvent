using EvoEvent.Domain.Entities;

namespace EvoEvent.Application.Abstractions
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
