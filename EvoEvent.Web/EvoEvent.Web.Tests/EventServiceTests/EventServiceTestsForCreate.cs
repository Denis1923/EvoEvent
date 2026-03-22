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

		[Theory]
		[ClassData(typeof(EventModelTest))]
		public void Add_NewEvent_ReturnIsSuccess(
			string title,
			string description,
			DateTime startAt, 
			DateTime endAt)
		{
			var isDate = endAt > startAt;
			if (isDate)
			{
				Assert.True(isDate);
				return;
			}

			Event newEvent = new Event(title, description, startAt, endAt);

			var newEventId = _eventService.AddEvent(newEvent);
			var events = _eventService.GetAll();

			if (!events.Any())
			{
				Assert.Empty(events);
				return;
			}

			Assert.True(newEventId != Guid.Empty);
			Assert.Contains(events, evt => evt.Title.Contains(newEvent.Title));
		}
	}
}
