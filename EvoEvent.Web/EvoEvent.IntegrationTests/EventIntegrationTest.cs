using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EvoEvent.IntegrationTests
{
	internal class EventIntegrationTest : IAsyncLifetime
	{
		private readonly PostgreSqlContainer _conteiner = new PostgreSqlBuilder().Build();

		public async Task DisposeAsync()
		{	
			await _conteiner.DisposeAsync();
		}

		public async Task InitializeAsync()
		{
			await _conteiner.StartAsync();
		}

		private AppDbContext CreateContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
								.UseNpgsql(_conteiner.GetConnectionString())
								.Options;


			var context = new AppDbContext(options);
			context.Database.Migrate();
			return context;
		}

		private async Task ResetDataBaseAsync()
		{
			NpgsqlConnection.ClearAllPools();
			using var context = CreateContext();
			await context.Database.EnsureDeletedAsync();
			await context.Database.MigrateAsync();	
		}

		#region Create

		public async Task CreateEvent_SavesEventToDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = CreateContext();
			var eventRepository = new EventRepository(context);
			var newEvent = new Event(Guid.NewGuid(), "Концерт", "Описание: Рок-концерт", DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), 10);

			// Act
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = CreateContext();
			var expEvent = await verifyContext.Events.FirstAsync(e => e.Id == newEvent.Id);

			Assert.NotNull(expEvent);
			Assert.Equal(expEvent.Title, newEvent.Title);
		}

		#endregion

		#region Read

		public async Task GetEvents_EventId_ReturnsPagingEventsFromDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = CreateContext();
			var eventRepository = new EventRepository(context);
			var newEvent = new Event(Guid.NewGuid(), "Концерт 2", "Описание: Pop-концерт", DateTime.Now.AddDays(1), DateTime.Now.AddDays(5), 100);
			await eventRepository.SaveChangesAsync();
			await eventRepository.AddEventAsync(newEvent);

			// Act
			await using var verifyContext = CreateContext();
			var expEvent = await verifyContext.Events.FirstAsync(e => e.Id == newEvent.Id);

			// Assert
			Assert.NotNull(expEvent);
			Assert.Equal(expEvent.Title, newEvent.Title);
		}

		#endregion

		#region Update

		public async Task UpdateEvent_Event_UpdateEventsToDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = CreateContext();
			var eventRepository = new EventRepository(context);
			var newEvent = new Event(Guid.NewGuid(), "Концерт 3", "Описание: Rap-концерт", DateTime.Now.AddDays(1), DateTime.Now.AddDays(15), 200);
			var updEvent = new Event(null, title: "Rap-концерт", "Описание: Rap-концерт", DateTime.Now.AddDays(2), DateTime.Now.AddDays(25), 100);
			await eventRepository.SaveChangesAsync();
			await eventRepository.AddEventAsync(newEvent);

			// Act	
			await using var actContext = CreateContext();
			eventRepository = new EventRepository(actContext);
			var expEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);
			expEvent.Update(updEvent);
			await actContext.SaveChangesAsync();

			// Assert
			await using var verifyContext = CreateContext();
			var verifyEvent = await verifyContext.Events.FirstAsync(e => e.Id == newEvent.Id);

			// Assert
			Assert.NotNull(verifyEvent);
			Assert.Equal(verifyEvent.Title, updEvent.Title);
			Assert.Equal(verifyEvent.TotalSeats, updEvent.TotalSeats);
		}

		#endregion

		#region Delete

		public async Task DeleteEvent_Event_DeleteEventsFromDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = CreateContext();
			var eventRepository = new EventRepository(context);
			var newEvent = new Event(Guid.NewGuid(), "Концерт", "Описание: Рок-концерт", DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), 10);
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			// Act
			await using var actContext = CreateContext();
			eventRepository = new EventRepository(actContext);
			eventRepository.RemoveEvent(newEvent);
			await eventRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = CreateContext();
			var deleteEvent = await verifyContext.Events.FirstAsync(e => e.Id == newEvent.Id);

			Assert.Null(deleteEvent);
		}

		#endregion
	}
}
