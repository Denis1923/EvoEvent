using EvoEvent.Web.Models;
using EvoEvent.Web.Services;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForUpdate
	{
		private readonly IEventService _eventService;

		public EventServiceTestsForUpdate()
		{
			_eventService = new EventService();
		}

		[Theory]
		[InlineData("Концерт 10")]
		[InlineData("Концерт 50")]
		public void Update_Event_ReturnIsSuccess(string nameExp)
		{
			var updEvent = new Event("Концерт Nickelback", "Описание. Концерт Nickelback", DateTime.Parse("2026-03-21"), DateTime.Parse("2026-03-26"));
			var _events = _eventService.GetAll();
			var eventExp = _eventService.GetEventsAboutWhen(_events, nameExp)?.FirstOrDefault();

			if (eventExp is null)
			{
				Assert.Null(eventExp);
				return;
			}

			_eventService.Save(eventExp, updEvent);

			eventExp = _eventService.GetById(eventExp.Id);

			Assert.True(eventExp.Title == updEvent.Title);
			Assert.True(eventExp.Description == updEvent.Description);
			Assert.True(eventExp.StartAt == updEvent.StartAt);
			Assert.True(eventExp.EndAt == updEvent.EndAt);
		}
	}
}
