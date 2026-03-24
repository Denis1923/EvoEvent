using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Services;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForDelete
	{
		private readonly IEventService _eventService;
		private readonly EventModelTest _eventModelTest;

		public EventServiceTestsForDelete()
		{
			_eventService = new EventService();
			_eventModelTest = new EventModelTest();
		}

		[Theory]
		[InlineData("Концерт 10")]
		public void Delete_EventId_ReturnIsSuccess(string nameExp)
		{
			var _events = _eventService.GetAll();
			var eventExp = _eventService.GetEventsAboutWhen(_events, nameExp)?.FirstOrDefault();

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
