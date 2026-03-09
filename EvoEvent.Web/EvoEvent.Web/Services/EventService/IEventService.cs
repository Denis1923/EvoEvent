using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services
{
	public interface IEventService
	{
		IEnumerable<Event> GetAll();

		Event? GetById(Guid id);

		Guid AddEvent(Event evt);

		void Save(Event extUpd, Event updEvt);

		bool DeleteById(Guid id);
	}
}
