using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EvoEvent.IntegrationTests
{
	public class BookingIntegrationTest : IAsyncLifetime
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
			var expBooking = await verifyContext.Bookings.FirstAsync(e => e.Id == newBoooking.Id);

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
			var expBooking = await verifyContext.Bookings.FirstAsync(e => e.Id == newBoooking.Id);

			// Assert
			Assert.NotNull(expBooking);
			Assert.Equal(expBooking.EventId, newEvent.Id);
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
			var updBookings = await actContext.Bookings.Where(e => e.Id == newEvent.Id).ToArrayAsync();
			
			foreach (var booking in newBoookings)
				booking.Reject();

			await bookingRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = await CreateContext();
			var isAllRejectBookings = await verifyContext.Bookings.Where(e => e.EventId == newEvent.Id).AllAsync(b => b.Status == BookingStatus.Rejected);
			
			Assert.True(isAllRejectBookings);
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
			var expBookings = await actContext.Bookings.Where(b => b.EventId == newEvent.Id).ToListAsync();
			bookingRepository = new BookingRepository(actContext);

			foreach (var booking in expBookings)
				bookingRepository.RemoveBooking(booking);

			await bookingRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = await CreateContext();
			var deletBookings = await verifyContext.Bookings.Where(b => b.EventId == newEvent.Id).ToListAsync();

			Assert.True(!deletBookings.Any());
		}

		#endregion
	}
}
