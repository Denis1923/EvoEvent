using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EvoEvent.IntegrationTests
{
	public class EventIntegrationTest : IClassFixture<PostgreSqlFixture>
	{
		private readonly PostgreSqlContainer _postgres;

		public EventIntegrationTest(PostgreSqlFixture fixture)
		{
			_postgres = fixture.Container;
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
			eventRepository = new EventRepository(verifyContext);
			var expEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);

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
			var newEvents = new List<Event> {
				new Event(Guid.NewGuid(), "Концерт", "Описание: Рок-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 10),
				new Event(Guid.NewGuid(), "Выставка", "Описание: Выставка импрессионистов", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(4), 5),
				new Event(Guid.NewGuid(), "Лекция", "Описание: Лекция по истории искусств", DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(5),7),
				new Event(Guid.NewGuid(), "Спектакль", "Описание: Гамлет в театре драмы", DateTime.UtcNow.AddDays(4), DateTime.UtcNow.AddDays(6), 19),
				new Event(Guid.NewGuid(), "Мастер-класс", "Описание: Гончарное искусство", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(7), 20),
				new Event(Guid.NewGuid(), "Киносеанс", "Описание: Ночной киносеанс", DateTime.UtcNow.AddDays(6), DateTime.UtcNow.AddDays(8), 49),
				new Event(Guid.NewGuid(), "Конференция", "Описание: Научная конференция", DateTime.UtcNow.AddDays(7), DateTime.UtcNow.AddDays(9), 12, 10),
				new Event(Guid.NewGuid(), "Вечеринка", "Описание: Хэллоуин-вечеринка", DateTime.UtcNow.AddDays(8), DateTime.UtcNow.AddDays(10), 5),
				new Event(Guid.NewGuid(), "Семинар", "Описание: Маркетинговый семинар", DateTime.UtcNow.AddDays(9), DateTime.UtcNow.AddDays(11), 90),
				new Event(Guid.NewGuid(), "Фестиваль", "Описание: Джазовый фестиваль", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(12), 78),
				new Event(Guid.NewGuid(), "Тренинг", "Описание: Ораторское мастерство", DateTime.UtcNow.AddDays(11), DateTime.UtcNow.AddDays(13), 40),
				new Event(Guid.NewGuid(), "Квест", "Описание: Квест-комната 'Тайны особняка'", DateTime.UtcNow.AddDays(12), DateTime.UtcNow.AddDays(14), 30),
				new Event(Guid.NewGuid(), "Ярмарка", "Описание: Рождественская ярмарка", DateTime.UtcNow.AddDays(13), DateTime.UtcNow.AddDays(15), 34),
				new Event(Guid.NewGuid(), "Хакатон", "Описание: AI-хакатон", DateTime.UtcNow.AddDays(14), DateTime.UtcNow.AddDays(16), 25),
				new Event(Guid.NewGuid(), "Благотворительность", "Описание: Благотворительный забег", DateTime.UtcNow.AddDays(15), DateTime.UtcNow.AddDays(17), 45),
				new Event(Guid.NewGuid(), "Благотворительность 2", "Описание: Благотворительный забег", DateTime.UtcNow.AddDays(15), DateTime.UtcNow.AddDays(17), 0)
			};

			var countNewEvent = newEvents.Count;

			foreach (var evt in newEvents)
				await eventRepository.AddEventAsync(evt);

			await eventRepository.SaveChangesAsync();

			// Act
			await using var verifyContext = await CreateContext();
			eventRepository = new EventRepository(verifyContext);
			var allEvents = await eventRepository.GetEventsAsync();
			var pagingEvents = allEvents.Skip(0).Take(10).ToList();

			// Assert
			Assert.True(allEvents.Any());
			Assert.Equal(countNewEvent, allEvents.Count);
			Assert.Equal(10, pagingEvents.Count);
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
			eventRepository = new EventRepository(verifyContext);
			var verifyEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);

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
			eventRepository = new EventRepository(verifyContext);
			var deleteEvent = await eventRepository.GetEventByIdAsync(newEvent.Id);

			Assert.Null(deleteEvent);
		}

		#endregion
	}
}
