using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using EvoEvent.Web.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsUpdate : IDisposable
	{
		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IEventService _eventService;
		private readonly IBookingService _bookingService;

		public BookingServiceTestsUpdate()
		{
			var dbName = Guid.NewGuid().ToString();
			var services = new ServiceCollection();
			services.AddDbContext<AppDbContext>(options =>
				options.UseInMemoryDatabase(dbName));
			services.AddScoped<IEventService, EventService>();
			services.AddScoped<IBookingService, BookingService>();
			services.AddScoped<IEventRepository, EventRepository>();
			services.AddScoped<IBookingRepository, BookingRepository>();

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
		[InlineData("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d")]
		public async Task Confirm_BookingId_ReturnBooking(string eventIdstr)
		{
			var eventId = Guid.Parse(eventIdstr);
			var status = BookingStatus.Confirmed;

			var newBooking = await _bookingService.CreateBookingAsync(eventId);
			var booking = await _bookingService.GetBookingByIdAsync(newBooking.Id);
			booking.Confirm();

			Assert.True(booking.Status == status);
			Assert.True(booking.ProcessedAt.HasValue);
		}

		[Theory]
		[InlineData("1e2f3a4b-5c6d-4e7f-8a9b-0c1d2e3f4a5b")]
		public async Task Reject_BookingId_ReturnBooking(string eventIdstr)
		{
			var eventId = Guid.Parse(eventIdstr);
			var statusR = BookingStatus.Rejected;
			var statusP = BookingStatus.Pending;

			var evetnExp = await _eventService.GetByIdAsync(eventId);
			var newBooking = await _bookingService.CreateBookingAsync(eventId);
			newBooking.Reject();
			var newBooking2 = await _bookingService.CreateBookingAsync(eventId);

			Assert.True(newBooking.Status == statusR);
			Assert.True(newBooking2.Status == statusP);

		}
	}
}
