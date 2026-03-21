using EvoEvent.Web.Models;
using EvoEvent.Web.Services;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForCreate
	{
		private readonly IEventService _eventService;

		public EventServiceTestsForCreate()
		{
			_eventService = new EventService();
		}

		[Fact]
		public void Add_NewEvent_ReturnIsSuccess()
		{
			var newEvent = new Event("Аватар", "Кинопримьера", DateTime.Parse("2026-03-12"), DateTime.Parse("2026-03-22"));

			var newEventId = _eventService.AddEvent(newEvent);
			var events = _eventService.GetAll();

			Assert.True(newEventId != Guid.Empty);
			Assert.Contains(events, evt => evt.Title.Contains(newEvent.Title));
		}
	}
}
