using EvoEvent.Web.Exceptions;
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
		[InlineData("57577abb-0603-45a0-9c51-498dbfd9a340")]
		public void Delete_EventId_ReturnIsSuccess(string entityIdstr)
		{			
			if (!Guid.TryParse(entityIdstr, out Guid entityId))
				Assert.True(entityId == Guid.Empty);

			var exc = Assert.Throws<NotFoundException>(
				() => _eventService.GetById(entityId));

			if (exc != null)
			{
				Assert.Equal($"Не найдено событие с таким ИД {entityId}", exc?.Message);
				return;
			}

			Assert.True(_eventService.DeleteById(entityId));
		}
	}
}
