using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using EvoEvent.Web.Services;
using EvoEvent.Web.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForCreate: IDisposable
	{
		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IEventService _eventService;

		public EventServiceTestsForCreate()
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
		[InlineData("Концерт", "Концерт в Москве", "2026-10-23", "2026-10-22")]
		public async Task Add_NewEvent_ReturnValidationException(
			string title,
			string description,
			string startAt,
			string endAt)
		{
			Event newEvent = new Event(
				Guid.NewGuid(), 
				title, 
				description, 
				DateTime.Parse(startAt), 
				DateTime.Parse(endAt),
				20);

			var exc = await Assert.ThrowsAsync<ValidationException>(
				async () => await _eventService.AddEventAsync(newEvent));

			Assert.Equal($"Дата окончания должна быть позже Даты начала", exc.Message);
		}

		[Theory]
		[InlineData("Концерт", "Концерт в Москве", "2026-10-22", "2026-10-26")]
		public async Task Add_NewEvent_ReturnIsSuccess(
			string title,
			string description,
			string startAt,
			string endAt)
		{
			Event newEvent = new Event(
				Guid.NewGuid(),
				title,
				description,
				DateTime.Parse(startAt),
				DateTime.Parse(endAt),
				20);

			var newEventId = await _eventService.AddEventAsync(newEvent);
			var events = await _eventService.GetAllAsync();

			Assert.True(newEventId != Guid.Empty);
			Assert.Contains(events, evt => evt.Id == newEventId);
		}
	}
}
