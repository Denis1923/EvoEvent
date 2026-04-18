using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using EvoEvent.Web.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsUpdate
	{
		private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
		private readonly Mock<IServiceScope> _mockScope;
		private readonly Mock<IServiceProvider> _mockServiceProvider;
		private readonly Mock<IEventService> _mockEventService;
		private readonly IEventService _eventService;
		private readonly IBookingService _bookingService;

		public BookingServiceTestsUpdate()
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
		[InlineData("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b")]
		public async Task Update_BookingId_ReturnBooking(string eventIdstr)
		{
			var eventId = Guid.Parse(eventIdstr);
			var status = BookingStatus.Confirmed;

			var expectedEvent = new Event();

			_mockEventService
				.Setup(es => es.GetById(eventId))
				.Returns(expectedEvent);

			var newBooking = await _bookingService.CreateBookingAsync(eventId);
			var booking = await _bookingService.GetBookingByIdAsync(newBooking.Id);

			booking.Status = BookingStatus.Confirmed;
			booking = await _bookingService.GetBookingByIdAsync(booking.Id);

			Assert.True(booking.Status == status);
		}
	}
}
