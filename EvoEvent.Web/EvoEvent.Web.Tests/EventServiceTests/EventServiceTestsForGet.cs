using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using Moq;
using System.Reflection;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForGet
	{
		private readonly IEventService _eventService;

		public EventServiceTestsForGet()
		{
			_eventService = new EventService();
		}

		[Fact]
		public void Get_ReturnsEvents()
		{
			var nameTitleExp = "Концерт #3";

			var events = _eventService.GetAll();

			Assert.NotEmpty(events);
			Assert.Contains(events, evt => evt.Title.Contains(nameTitleExp));
		}

		[Fact]
		public void Get_EventId_ReturnEvent()
		{
			var eventExp = _eventService.GetAll()?.First(); //так как Guid генерится системой, то для примера возьму любой первый элемент

			var eventObj = _eventService.GetById(eventExp.Id);

			Assert.NotNull(eventObj);
		}

		[Fact]
		public void Filter_EventName_ReturnEvents()
		{
			var nameExp = "Концерт #10";
			var nameNoExp = "Концерт #0";
			var _events = _eventService.GetAll();

			var events = _eventService.GetAllAboutWhen(_events, nameExp);

			Assert.All(events, evt => evt.Title.Contains(nameExp));
			Assert.DoesNotContain(nameNoExp, events.Select(e => e.Title));
		}

		[Fact]
		public void Filter_EventDates_ReturnEvents()
		{
			var dateStart = DateTime.Parse("2026-03-20");
			var dateEnd = DateTime.Parse("2026-03-30");
			var nameExp = "Концерт";
			var _events = _eventService.GetAll();

			var events = _eventService.GetAllAboutWhen(_events, string.Empty, dateStart, dateEnd);

			Assert.NotEmpty(events);
			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}

		[Fact]
		public void Paginated_PageAndPageSize_ReturnEvents()
		{
			var nameExp = "Концерт #19";
			var _events = _eventService.GetAll();

			var events = _eventService.GetAllAboutWhen(_events, string.Empty, null, null, 2, 10);

			Assert.NotEmpty(events);
			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}

		[Fact]
		public void Filters_ReturnEvents()
		{
			var title = "Концерт";
			var dateStart = DateTime.Parse("2026-03-20");
			var dateEnd = DateTime.Parse("2026-03-30");
			var nameExp = "Концерт";
			var _events = _eventService.GetAll();

			var events = _eventService.GetAllAboutWhen(_events, title, dateStart, dateEnd, 2, 10);

			Assert.NotEmpty(events);
			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}
	}
}
