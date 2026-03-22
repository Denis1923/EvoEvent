using EvoEvent.Web.Services;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForDelete
	{
		private readonly IEventService _eventService;

		public EventServiceTestsForDelete()
		{
			_eventService = new EventService();
		}

		[Theory]
		[InlineData("Концерт 10")]
		[InlineData("Концерт 25")]
		public void Delete_EventId_ReturnIsSuccess(string nameExp)
		{
			var _events = _eventService.GetAll();
			var eventExp = _eventService.GetEventsAboutWhen(_events, nameExp)?.FirstOrDefault();

			if (eventExp is null)
			{
				Assert.Null(eventExp);
				return;
			}

			_eventService.DeleteById(eventExp.Id);
			eventExp = _eventService.GetById(eventExp.Id);

			Assert.Null(eventExp);
		}
	}
}
