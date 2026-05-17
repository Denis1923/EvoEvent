using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EvoEvent.IntegrationTests
{
	public class EventIntegrationTest : IAsyncLifetime
	{
		private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
															.WithImage("postgres:16-alpine")
															.WithDatabase("evoevent")
															.Build();

		public async Task DisposeAsync()
		{
			await _postgres.DisposeAsync();
		}

		public async Task InitializeAsync()
		{
			await _postgres.StartAsync();
		}

		private async Task<AppDbContext> CreateContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
								.UseNpgsql(_postgres.GetConnectionString())
								.Options;


			var context = new AppDbContext(options);
			await context.Database.MigrateAsync();
			return context;
		}

		private async Task ResetDataBaseAsync()
		{
			NpgsqlConnection.ClearAllPools();
			using var context = await CreateContext();
			await context.Database.EnsureDeletedAsync();
			await context.Database.MigrateAsync();
		}

		#region Create

		[Fact]
		public async Task CreateEvent_SavesEventToDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var eventRepository = new EventRepository(context);
			var newEvent = new Event(Guid.NewGuid(), "Концерт", "Описание: Рок-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 10);

			// Act
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = await CreateContext();
			var expEvent = await verifyContext.Events.FirstAsync(e => e.Id == newEvent.Id);

			Assert.NotNull(expEvent);
			Assert.Equal(expEvent.Title, newEvent.Title);
		}

		#endregion

		#region Read

		[Fact]
		public async Task GetEvents_EventId_ReturnsPagingEventsFromDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var eventRepository = new EventRepository(context);
			var newEvent = new Event(Guid.NewGuid(), "Концерт 2", "Описание: Pop-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5), 100);
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			// Act
			await using var verifyContext = await CreateContext();
			var expEvent = await verifyContext.Events.FirstAsync(e => e.Id == newEvent.Id);

			// Assert
			Assert.NotNull(expEvent);
			Assert.Equal(expEvent.Title, newEvent.Title);
		}

		#endregion

		#region Update

		[Fact]
		public async Task UpdateEvent_Event_UpdateEventsToDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var eventRepository = new EventRepository(context);
			var newEvent = new Event(Guid.NewGuid(), "Концерт 3", "Описание: Rap-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(15), 200);
			var updEvent = new Event(null, title: "Rap-концерт", "Описание: Rap-концерт", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(25), 100);
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			// Act	
			await using var actContext = await CreateContext();
			eventRepository = new EventRepository(actContext);
			var expEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);
			expEvent.Update(updEvent);
			await actContext.SaveChangesAsync();

			// Assert
			await using var verifyContext = await CreateContext();
			var verifyEvent = await verifyContext.Events.FirstAsync(e => e.Id == newEvent.Id);

			// Assert
			Assert.NotNull(verifyEvent);
			Assert.Equal(verifyEvent.Title, updEvent.Title);
			Assert.Equal(verifyEvent.TotalSeats, updEvent.TotalSeats);
		}

		#endregion

		#region Delete

		[Fact]
		public async Task DeleteEvent_Event_DeleteEventsFromDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var eventRepository = new EventRepository(context);
			var newEvent = new Event(Guid.NewGuid(), "Концерт", "Описание: Рок-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 10);
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			// Act
			await using var actContext = await CreateContext();
			eventRepository = new EventRepository(actContext);
			eventRepository.RemoveEvent(newEvent);
			await eventRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = await CreateContext();
			var deleteEvent = await verifyContext.Events.FirstOrDefaultAsync(e => e.Id == newEvent.Id);

			Assert.Null(deleteEvent);
		}

		#endregion
	}
}
