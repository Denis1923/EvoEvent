using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using Moq;
using System.Reflection;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForUpdate
	{
		private readonly IEventService _eventService;

		public EventServiceTestsForUpdate()
		{
			_eventService = new EventService();
		}

		[Fact]
		public void Update_Event_ReturnIsSuccess()
		{
			var updEvent = new Event("Концерт Nickelback", "Описание. Концерт Nickelback", DateTime.Parse("2026-03-21"), DateTime.Parse("2026-03-26"));

			var eventExp = _eventService.GetAll()?.First(); //так как Guid генерится системой, то для примера возьму любой первый элемент

			_eventService.Save(eventExp, updEvent);

			eventExp = _eventService.GetById(eventExp.Id);

			Assert.True(eventExp.Title == updEvent.Title);
			Assert.True(eventExp.Description == updEvent.Description);
			Assert.True(eventExp.StartAt == updEvent.StartAt);
			Assert.True(eventExp.EndAt == updEvent.EndAt);
		}
	}
}
