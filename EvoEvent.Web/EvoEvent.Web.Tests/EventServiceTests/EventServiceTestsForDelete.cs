using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Repositories;
using EvoEvent.Web.Services;
using EvoEvent.Web.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForDelete : IDisposable
	{
		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IEventService _eventService;

		public EventServiceTestsForDelete()
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
		[InlineData("Спектакль")]
		public async Task Delete_EventId_ReturnIsSuccess(string nameExp)
		{
			var events = await _eventService.GetAllAsync();
			var eventExp = _eventService.GetEventsAboutWhen(events, nameExp)?.FirstOrDefault();

			var isDelete = await _eventService.DeleteByIdAsync(eventExp.Id);

			Assert.True(isDelete);
		}

		[Fact]
		public async Task Delete_EventId_ReturnIsNoSuccess()
		{			
			var entityId = Guid.NewGuid();
			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _eventService.GetByIdAsync(entityId));

			Assert.Equal($"Не найдено событие с таким ИД {entityId}", exc?.Message);
		}
	}
}
