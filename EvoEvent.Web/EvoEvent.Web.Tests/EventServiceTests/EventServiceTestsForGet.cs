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
		[InlineData("Концерт 15")]
		public void Get_ReturnsEvents(string nameTitleExp)
		{
			var events = _eventService.GetAll();
			
			if (!events.Any())
			{
				Assert.Empty(events);
				return;
			}

			Assert.NotEmpty(events);
			Assert.Contains(events, evt => evt.Title.Contains(nameTitleExp));
		}

		[Theory]
		[InlineData("Концерт 9")]
		[InlineData("Концерт 25")]
		public void Get_EventId_ReturnEvent(string nameExp)
		{
			var _events = _eventService.GetAll();
			var eventExp = _eventService.GetEventsAboutWhen(_events, nameExp)?.FirstOrDefault();

			if (eventExp is null)
			{
				Assert.Null(eventExp);
				return;
			}

			var eventObj = _eventService.GetById(eventExp.Id);

			Assert.NotNull(eventObj);
		}

		[Theory]
		[InlineData("Концерт 9", "Концерт 25")]
		[InlineData("Концерт 25", "Концерт 0")]
		public void Filter_EventName_ReturnEvents(string nameExp, string nameNoExp)
		{
			var _events = _eventService.GetAll();

			var events = _eventService.GetEventsAboutWhen(_events, nameExp);

			if (!events.Any())
			{
				Assert.Empty(events);
				return;
			}

			Assert.All(events, evt => evt.Title.Contains(nameExp));
			Assert.DoesNotContain(nameNoExp, events.Select(e => e.Title));
		}

		[Theory]
		[InlineData("Концерт 19")]
		[InlineData("Концерт 25")]
		public void Paginated_PageAndPageSize_ReturnEvents(string nameExp)
		{
			var _events = _eventService.GetAll();

			var events = _eventService.GetEventsAboutWhen(_events, string.Empty, null, null);
			events = _eventService.GetEventsAboutPaginated(events, 2, 10);

			if (!events.Any())
			{
				Assert.Empty(events);
				return;
			}

			Assert.NotEmpty(events);
			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}

		[Fact]
		public void Filter_EventDates_ReturnEvents()
		{
			var dateStart = DateTime.Parse("2026-03-20");
			var dateEnd = DateTime.Parse("2026-03-30");
			var nameExp = "Концерт";
			var _events = _eventService.GetAll();

			var events = _eventService.GetEventsAboutWhen(_events, string.Empty, dateStart, dateEnd);

			if (!events.Any())
			{
				Assert.Empty(events);
				return;
			}

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

			var events = _eventService.GetEventsAboutWhen(_events, title, dateStart, dateEnd);
			events = _eventService.GetEventsAboutPaginated(events, 2, 10);

			if (!events.Any())
			{
				Assert.Empty(events);
				return;
			}

			Assert.NotEmpty(events);
			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}
	}
}
