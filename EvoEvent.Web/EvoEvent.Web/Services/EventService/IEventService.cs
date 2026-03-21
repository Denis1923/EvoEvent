using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services
{
	public interface IEventService
	{
		IEnumerable<Event> GetAll();

		IEnumerable<Event> GetAllAboutWhen(
			IEnumerable<Event> events, 
			string? title = null, 
			DateTime? from = null, 
			DateTime? to = null, 
			int page = 1, 
			int pageSize = 10);

		Event? GetById(Guid id);

		Guid AddEvent(Event evt);

		void Save(Event extUpd, Event updEvt);

		bool DeleteById(Guid id);
	}
}
