using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
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
		private readonly EventModelTest eventModelTest;

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
		}

		[Theory]
		[InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
		public async void CreateBookingByEventId_ReturnIsStatusPending(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var expectedEvent = new Event();

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns(expectedEvent);

			var newBooking = await _bookingService.CreateBookingAsync(eventId);

			Assert.NotNull(newBooking);
			Assert.True(newBooking.Status == BookingStatus.Pending);
		}

		[Theory]
		[InlineData("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b")]
		public async void CreateBookingsByEventId_ReturnIsSuccess(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var countBooking = 5;
			var idsNewBooking = new List<Guid>();

			var expectedEvent = new Event();

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns(expectedEvent);

			for (int i = 0; i < countBooking; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId);
				idsNewBooking.Add(newBooking.Id);
			}

			Assert.True(countBooking == idsNewBooking.Distinct().Count());
		}

		[Fact]
		public async void Add_NewBooking_ReturnValidationException()
		{
			var eventId = Guid.Empty;

			var exc = await Assert.ThrowsAsync<ValidationException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"Передан не валидный параметр eventId = {eventId}", exc?.Message);
		}

		[Theory]
		[InlineData("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a9b")]
		public async void Add_NewBooking_ReturnNotFoundEvent(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"Не найдено событие с таким ИД {eventId}", exc?.Message);
		}

		[Theory]
		[InlineData("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b")]
		public async void Add_NewBooking_ReturnNotFoundDeleteEvent(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);

			var eventExc = _eventService.GetById(eventId);
			_eventService.DeleteById(eventId);

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.CreateBookingAsync(eventId));

			Assert.Equal($"Не найдено событие с таким ИД {eventId}", exc?.Message);
		}
	}
}