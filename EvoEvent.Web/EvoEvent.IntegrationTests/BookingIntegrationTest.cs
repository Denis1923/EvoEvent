using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EvoEvent.IntegrationTests
{
	public class BookingIntegrationTest : IClassFixture<PostgreSqlFixture>
	{
		private readonly PostgreSqlContainer _postgres;

		public BookingIntegrationTest(PostgreSqlFixture fixture)
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
		public async Task CreateBooking_SavesBookingToDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var eventRepository = new EventRepository(context);
			var bookingRepository = new BookingRepository(context);
			
			var newEvent = new Event(Guid.NewGuid(), "Концерт", "Описание: Рок-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 10);
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			var newBoooking = new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow);

			// Act
			await bookingRepository.AddBookingAsync(newBoooking);
			await bookingRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = await CreateContext();
			bookingRepository = new BookingRepository(verifyContext);
			var expBooking = await bookingRepository.GetBookingByIdAsync(newBoooking.Id);

			Assert.NotNull(expBooking);
			Assert.Equal(expBooking.EventId, newEvent.Id);
		}

		#endregion

		#region Read

		[Fact]
		public async Task GetBooking_BookingId_ReturnsBookingFromDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var eventRepository = new EventRepository(context);
			var bookingRepository = new BookingRepository(context);

			var newEvent = new Event(Guid.NewGuid(), "Концерт", "Описание: Pop-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 190);
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			var newBoooking = new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow);
			await bookingRepository.AddBookingAsync(newBoooking);
			await bookingRepository.SaveChangesAsync();

			// Act
			await using var verifyContext = await CreateContext();
			bookingRepository = new BookingRepository(verifyContext);
			var expBooking = await bookingRepository.GetBookingByIdAsync(newBoooking.Id);

			// Assert
			Assert.NotNull(expBooking);
			Assert.Equal(expBooking.EventId, newEvent.Id);
		}


		[Fact]
		public async Task GetBookings_StatusPending_ReturnsBookingFromDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var eventRepository = new EventRepository(context);
			var bookingRepository = new BookingRepository(context);

			var newEvent = new Event(Guid.NewGuid(), "Концерт", "Описание: Pop-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 190);
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			var newBoookings = new List<Booking>
			{
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow),
				new Booking(newEvent.Id, BookingStatus.Rejected, DateTime.UtcNow),
				new Booking(newEvent.Id, BookingStatus.Confirmed, DateTime.UtcNow),
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow)
			};

			foreach (var booking in newBoookings)
				await bookingRepository.AddBookingAsync(booking);

			await bookingRepository.SaveChangesAsync();

			// Act
			await using var verifyContext = await CreateContext();
			bookingRepository = new BookingRepository(verifyContext);
			var bookingsPending = await bookingRepository.GetBookingsByStatusAsync(BookingStatus.Pending);

			// Assert
			Assert.Equal(2, bookingsPending.Count);
		}

		#endregion

		#region Update

		[Fact]
		public async Task UpdateBooking_StatusReject_UpdateBookingToDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var eventRepository = new EventRepository(context);
			var bookingRepository = new BookingRepository(context);

			var newEvent = new Event(Guid.NewGuid(), "Концерт", "Описание: Pop-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 190);
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			var newBoookings = new List<Booking>
			{
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow),
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow),
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow),
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow)
			};

			foreach (var booking in newBoookings)
				await bookingRepository.AddBookingAsync(booking);
			
			await bookingRepository.SaveChangesAsync();

			// Act
			await using var actContext = await CreateContext();
			bookingRepository = new BookingRepository(actContext);
			var updBookings = await bookingRepository.GetBookingsByEventIdAsync(newEvent.Id);

			foreach (var booking in updBookings)
				booking.Reject();

			await bookingRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = await CreateContext();
			bookingRepository = new BookingRepository(verifyContext);
			var rejectBookings = await bookingRepository.GetBookingsByEventIdAsync(newEvent.Id);

			Assert.True(rejectBookings.All(b => b.Status == BookingStatus.Rejected));
		}

		#endregion

		#region Delete

		[Fact]
		public async Task DeleteBooking_Event_DeleteBookingsFromDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var eventRepository = new EventRepository(context);
			var bookingRepository = new BookingRepository(context);

			var newEvent = new Event(Guid.NewGuid(), "Концерт", "Описание: Hous-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 90);
			await eventRepository.AddEventAsync(newEvent);
			await eventRepository.SaveChangesAsync();

			var newBoookings = new List<Booking>
			{
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow),
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow),
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow),
				new Booking(newEvent.Id, BookingStatus.Pending, DateTime.UtcNow)
			};

			foreach (var booking in newBoookings)
				await bookingRepository.AddBookingAsync(booking);

			await bookingRepository.SaveChangesAsync();

			// Act
			await using var actContext = await CreateContext();
			bookingRepository = new BookingRepository(actContext);
			var expBookings = await bookingRepository.GetBookingsByEventIdAsync(newEvent.Id);

			foreach (var booking in expBookings)
				bookingRepository.RemoveBooking(booking);

			await bookingRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = await CreateContext();
			bookingRepository = new BookingRepository(verifyContext);
			var deleteBookings = await bookingRepository.GetBookingsByEventIdAsync(newEvent.Id);

			Assert.True(!deleteBookings.Any());
		}

		#endregion
	}
}
