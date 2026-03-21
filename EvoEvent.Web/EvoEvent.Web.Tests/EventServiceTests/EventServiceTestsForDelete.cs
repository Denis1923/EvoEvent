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

		[Fact]
		public void Delete_EventId_ReturnIsSuccess()
		{
			var eventExp = _eventService.GetAll()?.First(); //так как Guid генерится системой, то для примера возьму любой первый элемент

			_eventService.DeleteById(eventExp.Id);
			eventExp = _eventService.GetById(eventExp.Id);

			Assert.Null(eventExp);
		}
	}
}
