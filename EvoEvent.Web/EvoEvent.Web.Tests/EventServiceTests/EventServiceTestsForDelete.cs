using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Services;
using EvoEvent.Web.Tests.Models;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForDelete
	{
		private readonly IEventService _eventService;

		public EventServiceTestsForDelete()
		{
			_eventService = new EventService();

			var events = ModelEventServiceTests.GetEvents();
			events.ForEach(evt => _eventService.AddEvent(evt));
		}

		[Theory]
		[InlineData("Спектакль")]
		public void Delete_EventId_ReturnIsSuccess(string nameExp)
		{
			var events = _eventService.GetAll();
			var eventExp = _eventService.GetEventsAboutWhen(events, nameExp)?.FirstOrDefault();

			var isDelete = _eventService.DeleteById(eventExp.Id);

			Assert.True(isDelete);
		}

		[Fact]
		public void Delete_EventId_ReturnIsNoSuccess()
		{			
			var entityId = Guid.NewGuid();
			var exc = Assert.Throws<NotFoundException>(
				() => _eventService.GetById(entityId));

			Assert.Equal($"Не найдено событие с таким ИД {entityId}", exc?.Message);
		}
	}
}
