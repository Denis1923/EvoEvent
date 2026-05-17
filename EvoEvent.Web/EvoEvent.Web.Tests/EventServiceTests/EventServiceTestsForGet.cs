using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Repositories;
using EvoEvent.Web.Services;
using EvoEvent.Web.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForGet : IDisposable
	{
		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IEventService _eventService;

		public EventServiceTestsForGet()
		{
			var dbName = Guid.NewGuid().ToString();
			var services = new ServiceCollection();
			services.AddDbContext<AppDbContext>(options =>
				options.UseInMemoryDatabase(dbName));
			services.AddScoped<IEventService, EventService>();
			services.AddScoped<IEventRepository, EventRepository>();

			_serviceProvider = services.BuildServiceProvider();
			_scope = _serviceProvider.CreateScope();
			_eventService = _scope.ServiceProvider.GetRequiredService<IEventService>();

			var events = ModelEventServiceTests.GetEvents();
			events.ForEach(evt => _eventService.AddEventAsync(evt));
		}

		public void Dispose()
		{
			_scope.Dispose();
			_serviceProvider.Dispose();
		}

		[Theory]
		[InlineData("Хакатон")]
		public async Task Get_ReturnsEvents(string nameTitleExp)
		{
			var events = await _eventService.GetAllAsync();
			
			Assert.NotEmpty(events);
			Assert.Contains(events, evt => evt.Title.Contains(nameTitleExp));
		}

		[Theory]
		[InlineData("Хакатон")]
		public async Task Get_EventId_ReturnEvent(string nameExp)
		{
			var events = await _eventService.GetAllAsync();
			var eventExp = _eventService.GetEventsAboutWhen(events, nameExp)?.FirstOrDefault();

			var eventObj = await _eventService.GetByIdAsync(eventExp.Id);

			Assert.NotNull(eventObj);
		}

		[Fact]
		public async Task Get_EventId_ReturnNoEvent()
		{
			var entityId = Guid.NewGuid();

			var exc = await Assert.ThrowsAsync<NotFoundException>( 
				async () => await _eventService.GetByIdAsync(entityId));

			Assert.Equal($"Не найдено событие с таким ИД {entityId}", exc?.Message);
		}

		[Theory]
		[InlineData("Фестиваль", "Концерт 25")]
		public async Task Filter_EventName_ReturnEvents(string nameExp, string nameNoExp)
		{
			var events = await _eventService.GetAllAsync();
			events = _eventService.GetEventsAboutWhen(events, nameExp);

			Assert.All(events, evt => evt.Title.Contains(nameExp));
			Assert.DoesNotContain(nameNoExp, events.Select(e => e.Title));
		}

		[Fact]
		public async Task Paginated_PageAndPageSize_ReturnEvents()
		{
			var events = await _eventService.GetAllAsync();
			events = _eventService.GetEventsAboutPaginated(events, 2, 10);

			Assert.NotEmpty(events);
		}

		[Fact]
		public async Task Filter_EventDates_ReturnEvents()
		{
			var dateStart = DateTime.UtcNow;
			var dateEnd = DateTime.UtcNow.AddDays(4);
			var nameExp = "Фестиваль";
			var _events = await _eventService.GetAllAsync();

			var events = _eventService.GetEventsAboutWhen(_events, string.Empty, dateStart, dateEnd);
			
			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}

		[Fact]
		public async Task Filters_ReturnEvents()
		{
			var title = "Тренинг";
			var dateStart = DateTime.UtcNow;
			var dateEnd = DateTime.UtcNow.AddDays(14);
			var nameExp = "Тренинг";
			var _events = await _eventService.GetAllAsync();

			var events = _eventService.GetEventsAboutWhen(_events, title, dateStart, dateEnd);
			events = _eventService.GetEventsAboutPaginated(events, 2, 10);

			Assert.All(events, evt => evt.Title.Contains(nameExp));
		}
	}
}
