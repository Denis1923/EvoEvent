using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Services;
using EvoEvent.Web.Tests.Models;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForGet
	{
		private readonly IEventService _eventService;

		public EventServiceTestsForGet()
		{
			_eventService = new EventService();

			var events = ModelEventServiceTests.GetEvents();
			events.ForEach(evt => _eventService.AddEvent(evt));
		}

		[Theory]
		[InlineData("Хакатон")]
		public void Get_ReturnsEvents(string nameTitleExp)
		{
			var events = _eventService.GetAll();
			
			Assert.NotEmpty(events);
			Assert.Contains(events, evt => evt.Title.Contains(nameTitleExp));
		}

		[Theory]
		[InlineData("Хакатон")]
		public void Get_EventId_ReturnEvent(string nameExp)
		{
			var events = _eventService.GetAll();
			var eventExp = _eventService.GetEventsAboutWhen(events, nameExp)?.FirstOrDefault();

			var eventObj = _eventService.GetById(eventExp.Id);

			Assert.NotNull(eventObj);
		}

		[Fact]
		public void Get_EventId_ReturnNoEvent()
		{
			var entityId = Guid.NewGuid();

			var exc = Assert.Throws<NotFoundException>( 
				() =>  _eventService.GetById(entityId));

			Assert.Equal($"Не найдено событие с таким ИД {entityId}", exc?.Message);
		}

		[Theory]
		[InlineData("Фестиваль", "Концерт 25")]
		public void Filter_EventName_ReturnEvents(string nameExp, string nameNoExp)
		{
			var events = _eventService.GetAll();
			events = _eventService.GetEventsAboutWhen(events, nameExp);

			Assert.All(events, evt => evt.Title.Contains(nameExp));
			Assert.DoesNotContain(nameNoExp, events.Select(e => e.Title));
		}

		[Fact]
		public void Paginated_PageAndPageSize_ReturnEvents()
		{
			var events = _eventService.GetAll();
			events = _eventService.GetEventsAboutPaginated(events, 2, 10);

			Assert.NotEmpty(events);
		}

		[Fact]
		public void Filter_EventDates_ReturnEvents()
		{
			var dateStart = DateTime.Now;
			var dateEnd = DateTime.Now.AddDays(4);
			var nameExp = "Фестиваль";
			var _events = _eventService.GetAll();

			var events = _eventService.GetEventsAboutWhen(_events, string.Empty, dateStart, dateEnd);
			
			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}

		[Fact]
		public void Filters_ReturnEvents()
		{
			var title = "Тренинг";
			var dateStart = DateTime.Now;
			var dateEnd = DateTime.Now.AddDays(14);
			var nameExp = "Тренинг";
			var _events = _eventService.GetAll();

			var events = _eventService.GetEventsAboutWhen(_events, title, dateStart, dateEnd);
			events = _eventService.GetEventsAboutPaginated(events, 2, 10);

			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}
	}
}
