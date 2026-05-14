using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using EvoEvent.Web.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForUpdate : IDisposable
	{
		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IEventService _eventService;

		public EventServiceTestsForUpdate()
		{
			var dbName = Guid.NewGuid().ToString();
			var services = new ServiceCollection();
			services.AddDbContext<AppDbContext>(options =>
				options.UseInMemoryDatabase(dbName));
			services.AddScoped<IEventService, EventService>();

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
		[InlineData("Квест")]
		public async Task Update_Event_ReturnIsSuccess(string nameExp)
		{
			var updEvent = new Event(
				null, 
				"Концерт Nickelback", 
				"Описание. Концерт Nickelback", 
				DateTime.Now, 
				DateTime.Now.AddDays(4),
				20);

			var _events = await _eventService.GetAllAsync();
			var eventExp = _eventService.GetEventsAboutWhen(_events, nameExp)?.FirstOrDefault();

			_eventService.UpdateEvent(eventExp, updEvent);

			Assert.True(eventExp.Title == updEvent.Title);
			Assert.True(eventExp.Description == updEvent.Description);
			Assert.True(eventExp.StartAt == updEvent.StartAt);
			Assert.True(eventExp.EndAt == updEvent.EndAt);
		}

		[Fact]
		public async Task Update_Event_ReturnIsNoSuccess()
		{
			var entityId = Guid.NewGuid();

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _eventService.GetByIdAsync(entityId));

			Assert.Equal($"Не найдено событие с таким ИД {entityId}", exc?.Message);
		}
	}
}
