using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using EvoEvent.Web.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsCreate : IDisposable
	{
		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IEventService _eventService;
		private readonly IBookingService _bookingService;

		public BookingServiceTestsCreate()
		{
			var dbName = Guid.NewGuid().ToString();
			var services = new ServiceCollection();
			services.AddDbContext<AppDbContext>(options =>
				options.UseInMemoryDatabase(dbName));
			services.AddScoped<IEventService, EventService>();
			services.AddScoped<IBookingService, BookingService>();

			_serviceProvider = services.BuildServiceProvider();
			_scope = _serviceProvider.CreateScope();
			_eventService = _scope.ServiceProvider.GetRequiredService<IEventService>();
			_bookingService = _scope.ServiceProvider.GetRequiredService<IBookingService>();

			var events = ModelEventServiceTests.GetEvents();
			events.ForEach(evt => _eventService.AddEventAsync(evt));
		}

		public void Dispose()
		{
			_scope.Dispose();
			_serviceProvider.Dispose();
		}

		[Theory]
		[InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
		public async Task CreateBookingByEventId_ReturnIsStatusPending(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);

			var eventExp = await _eventService.GetByIdAsync(eventId);
			var newBooking = await _bookingService.CreateBookingAsync(eventId);

			Assert.NotNull(newBooking);
			Assert.True(newBooking.Status == BookingStatus.Pending);
			Assert.Equal(eventExp.AvailableSeats, eventExp.TotalSeats - 1);
		}

		[Theory]
		[InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
		public async Task CreateBookingsByEventId_ReturnIsSuccess(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var idsNewBooking = new List<Guid>();

			var eventExp = await _eventService.GetByIdAsync(eventId);

			for (int i = 0; i < eventExp.TotalSeats; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId);
				idsNewBooking.Add(newBooking.Id);
			}

			Assert.Equal(eventExp.TotalSeats, idsNewBooking.Distinct().Count());
			Assert.Equal(eventExp.AvailableSeats, 0);
		}

		[Theory]
		[InlineData("b1c4a9e3-7d2f-4a6e-8b5c-9e2d1f3a4b6c")]
		public async Task CreateBookingsByEventId_ReturnNoAvailableSeats(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var idsNewBooking = new List<Guid>();

			var eventExp = await _eventService.GetByIdAsync(eventId);

			for (int i = 0; i < eventExp.TotalSeats; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId);
				idsNewBooking.Add(newBooking.Id);
			}

			var exc = await Assert.ThrowsAsync<NoAvailableSeatsException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"No available seats for this event", exc?.Message);
			Assert.Equal(eventExp.TotalSeats, idsNewBooking.Distinct().Count());
		}

		[Fact]
		public async Task Add_NewBooking_ReturnValidationException()
		{
			var eventId = Guid.Empty;

			var exc = await Assert.ThrowsAsync<ValidationException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"Передан не валидный параметр eventId = {eventId}", exc?.Message);
		}

		[Theory]
		[InlineData("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a9b")]
		public async Task Add_NewBooking_ReturnNotFoundEvent(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"Не найдено событие с таким ИД {eventId}", exc?.Message);
		}

		[Theory]
		[InlineData("4f5e6d7c-8b9a-4e0f-1d2c-3a4b5c6d7e82")]
		public async Task Add_NewBooking_ReturnNotFoundDeleteEvent(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"Не найдено событие с таким ИД {eventId}", exc?.Message);
		}

		[Theory]
		[InlineData("8d8e9f0a-1b2c-4d3e-5f6a-7b8c9d0e1f2a")]
		public async Task Add_NewBooking_ReturnNoAvailableSeats(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);

			var exc = await Assert.ThrowsAsync<NoAvailableSeatsException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"No available seats for this event", exc?.Message);
		}

		[Fact]
		public async Task CreateBookingAsync_ConcurrentRequests_DoesNotOverbookEvent()
		{
			const int totalSeats = 5;
			const int concurrentRequests = 20;
			var eventExp = await _eventService.GetByIdAsync(Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"));

			var tasks = Enumerable.Range(0, concurrentRequests)
				.Select(_ => Task.Run(async () =>
				{
					using var scope = _serviceProvider.CreateScope();
					var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
					try
					{
						await bookingService.CreateBookingAsync(eventExp.Id);
						return true;
					}
					catch (NoAvailableSeatsException)
					{
						return false;
					}
				}));

			var results = await Task.WhenAll(tasks);

			var successCount = results.Count(r => r);
			Assert.Equal(totalSeats, successCount);
		}

		[Theory]
		[InlineData("7c9e6679-7425-40de-944b-e07fc1f90ae7")]
		public async Task AddParralelBooking_ReturnBookings(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var idsNewBooking = new List<Guid>();
			var results = new ConcurrentBag<(bool Success, NoAvailableSeatsException Exception)>();

			var eventExp = await _eventService.GetByIdAsync(eventId);

			var options = new ParallelOptions
			{
				MaxDegreeOfParallelism = 20
			};

			await Parallel.ForEachAsync(Enumerable.Range(0, 20), options, async (_, _) =>
			{
				try
				{
					using var scope = _serviceProvider.CreateScope();
					var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
					await bookingService.CreateBookingAsync(eventId);

					results.Add((true, null));
				}
				catch (NoAvailableSeatsException ex)
				{
					results.Add((false, ex));
				}
			});

			// Assert
			var successFullCount = results.Count(r => r.Success);
			var noSeatsCount = results.Count(r => r.Exception is NoAvailableSeatsException);

			Assert.Equal(5, successFullCount);
			Assert.Equal(15, noSeatsCount);
			Assert.Equal(0, eventExp.AvailableSeats);
		}

		[Theory]
		[InlineData("9a8b7c6d-5e4f-4a3b-2c1d-0e9f8a7b6c5d")]
		public async Task AddParralelBooking_ReturnDistinctBookings(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var idsNewBooking = new List<Guid>();
			var results = new ConcurrentBag<Guid>();

			var eventExp = await _eventService.GetByIdAsync(eventId);

			var options = new ParallelOptions
			{
				MaxDegreeOfParallelism = 10
			};

			await Parallel.ForEachAsync(Enumerable.Range(0, 10), options, async (_, _) =>
			{
				using var scope = _serviceProvider.CreateScope();
				var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
				var newBooking = await bookingService.CreateBookingAsync(eventId);
				results.Add(newBooking.Id);
			});

			// Assert
			var distincCount = results.Distinct().Count();

			Assert.Equal(10, distincCount);
		}
	}
}