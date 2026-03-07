using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services
{
	public interface IEventService
	{
		IEnumerable<Event> GetAll();

		Event? GetById(Guid id);

		void AddEvent(EventDto evt);

		void Save(Guid id, EventDto evt);

		bool DeleteById(Guid id);
	}
}
