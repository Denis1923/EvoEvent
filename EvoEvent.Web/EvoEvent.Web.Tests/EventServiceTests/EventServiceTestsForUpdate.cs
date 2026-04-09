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
		[InlineData("Квест")]
		public void Update_Event_ReturnIsSuccess(string nameExp)
		{
			var updEvent = new Event(null, "Концерт Nickelback", "Описание. Концерт Nickelback", DateTime.Now, DateTime.Now.AddDays(4));
			var _events = _eventService.GetAll();
			var eventExp = _eventService.GetEventsAboutWhen(_events, nameExp)?.FirstOrDefault();

			_eventService.Save(eventExp, updEvent);

			Assert.True(eventExp.Title == updEvent.Title);
			Assert.True(eventExp.Description == updEvent.Description);
			Assert.True(eventExp.StartAt == updEvent.StartAt);
			Assert.True(eventExp.EndAt == updEvent.EndAt);
		}

		[Fact]
		public void Update_Event_ReturnIsNoSuccess()
		{
			var entityId = Guid.NewGuid();

			var exc = Assert.Throws<NotFoundException>(
				() => _eventService.GetById(entityId));

			Assert.Equal($"Не найдено событие с таким ИД {entityId}", exc?.Message);
		}
	}
}
