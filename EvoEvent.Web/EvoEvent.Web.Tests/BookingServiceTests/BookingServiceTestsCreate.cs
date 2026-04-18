using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using EvoEvent.Web.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsCreate
	{
		private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
		private readonly Mock<IServiceScope> _mockScope;
		private readonly Mock<IServiceProvider> _mockServiceProvider;
		private readonly Mock<IEventService> _mockEventService;
		private readonly IEventService _eventService;
		private readonly BookingService _bookingService;

		public BookingServiceTestsCreate()
		{
			_eventService = new EventService();
			_mockScopeFactory = new Mock<IServiceScopeFactory>();
			_mockScope = new Mock<IServiceScope>();
			_mockServiceProvider = new Mock<IServiceProvider>();
			_mockEventService = new Mock<IEventService>();

			// Базовая цепочка настроек
			_mockScopeFactory
				.Setup(f => f.CreateScope())
				.Returns(_mockScope.Object);

			_mockScope
				.Setup(s => s.ServiceProvider)
				.Returns(_mockServiceProvider.Object);

			_mockServiceProvider
				.Setup(sp => sp.GetService(typeof(IEventService)))
				.Returns(_mockEventService.Object);

			_bookingService = new BookingService(_mockScopeFactory.Object);

			var events = ModelEventServiceTests.GetEvents();
			events.ForEach(evt => _eventService.AddEvent(evt));
		}

		[Theory]
		[InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d499")]
		public async Task CreateBookingByEventId_ReturnIsStatusPending(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var expectedEvent = new Event(
				eventId,
				"Концерт 1", 
				"Описание: Рок-концерт",
				DateTime.Now.AddDays(1),
				DateTime.Now.AddDays(3), 
				10);

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns(expectedEvent);

			var newBooking = await _bookingService.CreateBookingAsync(eventId);

			Assert.NotNull(newBooking);
			Assert.True(newBooking.Status == BookingStatus.Pending);
			Assert.Equal(expectedEvent.AvailableSeats, expectedEvent.TotalSeats - 1);
		}

		[Theory]
		[InlineData("a4bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b")]
		public async Task CreateBookingsByEventId_ReturnIsSuccess(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var idsNewBooking = new List<Guid>();

			var expectedEvent = new Event(
				eventId,
				"Концерт 2",
				"Описание: Рок-концерт",
				DateTime.Now.AddDays(1),
				DateTime.Now.AddDays(3),
				10);

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns(expectedEvent);

			for (int i = 0; i < expectedEvent.TotalSeats; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId);
				idsNewBooking.Add(newBooking.Id);
			}

			Assert.Equal(expectedEvent.TotalSeats, idsNewBooking.Distinct().Count());
			Assert.Equal(expectedEvent.AvailableSeats, 0);
		}

		[Theory]
		[InlineData("a4bb4d2e-8f4d-4d6e-9f5c-3b6f7e0d9a0b")]
		public async Task CreateBookingsByEventId_ReturnNoAvailableSeats(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var idsNewBooking = new List<Guid>();

			var expectedEvent = new Event(
				eventId,
				"Концерт 2",
				"Описание: Рок-концерт",
				DateTime.Now.AddDays(1),
				DateTime.Now.AddDays(3),
				10);

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns(expectedEvent);

			for (int i = 0; i < expectedEvent.TotalSeats; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId);
				idsNewBooking.Add(newBooking.Id);
			}

			var exc = await Assert.ThrowsAsync<NoAvailableSeatsException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"No available seats for this event", exc?.Message);
			Assert.Equal(expectedEvent.TotalSeats, idsNewBooking.Distinct().Count());
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
		[InlineData("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b")]
		public async Task Add_NewBooking_ReturnNotFoundDeleteEvent(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns((Event)null);

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"Не найдено событие с таким ИД {eventId}", exc?.Message);
		}

		[Theory]
		[InlineData("53bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b")]
		public async Task Add_NewBooking_ReturnNoAvailableSeats(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns(new Event());

			var exc = await Assert.ThrowsAsync<NoAvailableSeatsException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"No available seats for this event", exc?.Message);
		}

		[Theory]
		[InlineData("53bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b")]
		public async Task AddParralelBooking_ReturnBookings(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var idsNewBooking = new List<Guid>();

			var expectedEvent = new Event(
				eventId,
				"Концерт 2",
				"Описание: Рок-концерт",
				DateTime.Now.AddDays(1),
				DateTime.Now.AddDays(3),
				5);

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns(expectedEvent);

			var results = new ConcurrentBag<(bool Success, NoAvailableSeatsException Exception)>();

			var options = new ParallelOptions
			{
				MaxDegreeOfParallelism = 20
			};

			await Parallel.ForEachAsync(Enumerable.Range(0, 20), options, async (_, _) =>
			{
				try
				{
					await _bookingService.CreateBookingAsync(eventId);
					results.Add((true, null));
				}
				catch (NoAvailableSeatsException ex)
				{
					results.Add((false, ex));
				}
			});

			// Assert
			var successfulCount = results.Count(r => r.Success);
			var noSeatsCount = results.Count(r => r.Exception is NoAvailableSeatsException);

			Assert.Equal(5, successfulCount);
			Assert.Equal(15, noSeatsCount);
			Assert.Equal(0, expectedEvent.AvailableSeats);
		}

		[Theory]
		[InlineData("53bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b")]
		public async Task AddParralelBooking_ReturnDistinctBookings(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var idsNewBooking = new List<Guid>();

			var expectedEvent = new Event(
				eventId,
				"Концерт 2",
				"Описание: Рок-концерт",
				DateTime.Now.AddDays(1),
				DateTime.Now.AddDays(3),
				10);

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns(expectedEvent);

			var results = new ConcurrentBag<Guid>();

			var options = new ParallelOptions
			{
				MaxDegreeOfParallelism = 10
			};

			await Parallel.ForEachAsync(Enumerable.Range(0, 10), options, async (_, _) =>
			{

				var newBooking = await _bookingService.CreateBookingAsync(eventId);
				results.Add(newBooking.Id);
			});

			// Assert
			var distincCount = results.Distinct().Count();

			Assert.Equal(10, distincCount);
		}
	}
}