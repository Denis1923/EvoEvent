using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EvoEvent.Web.Repositories
{
	public class EventRepository : IEventRepository
	{
		private readonly AppDbContext _context;

		public EventRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task AddEventAsync(Event newEvt, CancellationToken token)
			=> await _context.Events.AddAsync(newEvt, token);

		public async Task<Event?> GetEventByIdAsync(Guid id, CancellationToken token)
			=> await _context.Events.FirstOrDefaultAsync(e => e.Id == id, token);

		public async Task<List<Event>> GetEventsAsync()
			=> await _context.Events.ToListAsync();

		public void RemoveEvent(Event expEvt)
			=> _context.Events.Remove(expEvt);

		public async Task SaveChangesAsync(CancellationToken token)
			=> await _context.SaveChangesAsync();
	}
}
