using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Services;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForGet
	{
		private readonly IEventService _eventService;

		public EventServiceTestsForGet()
		{
			_eventService = new EventService();
		}

		[Theory]
		[InlineData("Концерт 9")]
		public void Get_ReturnsEvents(string nameTitleExp)
		{
			var exc = Assert.Throws<NotFoundException>(
				() => _eventService.GetAll());

			if (exc != null)
			{
				Assert.Equal($"Событий нет", exc?.Message);
				return;
			}

			var events = _eventService.GetAll();
			
			Assert.NotEmpty(events);
			Assert.Contains(events, evt => evt.Title.Contains(nameTitleExp));
		}

		[Theory]
		[InlineData("57577abb-0603-45a0-9c51-498dbfd9a340")]
		public void Get_EventId_ReturnEvent(string entityIdstr)
		{
			if (!Guid.TryParse(entityIdstr, out Guid entityId))
				Assert.True(entityId == Guid.Empty);

			var exc = Assert.Throws<NotFoundException>( 
				() =>  _eventService.GetById(entityId));

			Assert.Equal($"Не найдено событие с таким ИД {entityId}", exc?.Message);
		}

		[Theory]
		[InlineData("Концерт 9", "Концерт 25")]
		public void Filter_EventName_ReturnEvents(string nameExp, string nameNoExp)
		{
			var exc = Assert.Throws<NotFoundException>(
				() => _eventService.GetAll());

			if (exc != null)
			{
				Assert.Equal($"Событий нет", exc?.Message);
				return;
			}

			var _events = _eventService.GetAll();
			var events = _eventService.GetEventsAboutWhen(_events, nameExp);

			Assert.All(events, evt => evt.Title.Contains(nameExp));
			Assert.DoesNotContain(nameNoExp, events.Select(e => e.Title));
		}

		[Theory]
		[InlineData("Концерт 19")]
		public void Paginated_PageAndPageSize_ReturnEvents(string nameExp)
		{
			var exc = Assert.Throws<NotFoundException>(
				() => _eventService.GetAll());

			if (exc != null)
			{
				Assert.Equal($"Событий нет", exc?.Message);
				return;
			}

			var _events = _eventService.GetAll();

			var events = _eventService.GetEventsAboutWhen(_events, string.Empty, null, null);
			events = _eventService.GetEventsAboutPaginated(events, 2, 10);

			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}

		[Fact]
		public void Filter_EventDates_ReturnEvents()
		{
			var exc = Assert.Throws<NotFoundException>(
				() => _eventService.GetAll());

			if (exc != null)
			{
				Assert.Equal($"Событий нет", exc?.Message);
				return;
			}

			var dateStart = DateTime.Parse("2026-03-20");
			var dateEnd = DateTime.Parse("2026-03-30");
			var nameExp = "Концерт";
			var _events = _eventService.GetAll();

			var events = _eventService.GetEventsAboutWhen(_events, string.Empty, dateStart, dateEnd);
			
			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}

		[Fact]
		public void Filters_ReturnEvents()
		{
			var exc = Assert.Throws<NotFoundException>(
				() => _eventService.GetAll());

			if (exc != null)
			{
				Assert.Equal($"Событий нет", exc?.Message);
				return;
			}

			var title = "Концерт";
			var dateStart = DateTime.Parse("2026-03-20");
			var dateEnd = DateTime.Parse("2026-03-30");
			var nameExp = "Концерт";
			var _events = _eventService.GetAll();

			var events = _eventService.GetEventsAboutWhen(_events, title, dateStart, dateEnd);
			events = _eventService.GetEventsAboutPaginated(events, 2, 10);

			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}
	}
}
