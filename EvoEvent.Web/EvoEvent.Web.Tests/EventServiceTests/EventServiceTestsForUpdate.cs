using EvoEvent.Web.Exceptions;
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
		[InlineData("57577abb-0603-45a0-9c51-498dbfd9a340")]
		public void Update_Event_ReturnIsSuccess(string entityIdstr)
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

			var updEvent = new Event("Концерт Nickelback", "Описание. Концерт Nickelback", DateTime.Parse("2026-03-21"), DateTime.Parse("2026-03-26"));

			var eventExp = _eventService.GetById(entityId);
			_eventService.Save(eventExp, updEvent);

			Assert.True(eventExp.Title == updEvent.Title);
			Assert.True(eventExp.Description == updEvent.Description);
			Assert.True(eventExp.StartAt == updEvent.StartAt);
			Assert.True(eventExp.EndAt == updEvent.EndAt);
		}
	}
}
