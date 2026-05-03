using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using EvoEvent.Web.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsGet : IDisposable
	{

		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IBookingService _bookingService;
		private readonly IEventService _eventService;

		public BookingServiceTestsGet()
		{
			var dbName = Guid.NewGuid().ToString();
			var services = new ServiceCollection();
			services.AddDbContext<AppDbContext>(options =>
				options.UseInMemoryDatabase(dbName));
			services.AddScoped<IEventService, EventService>();
			services.AddScoped<IBookingService, BookingService>();

			_serviceProvider = services.BuildServiceProvider();
			_scope = _serviceProvider.CreateScope();
			_bookingService = _scope.ServiceProvider.GetRequiredService<IBookingService>();
			_eventService = _scope.ServiceProvider.GetRequiredService<IEventService>();

			var events = ModelEventServiceTests.GetEvents();
			events.ForEach(evt => _eventService.AddEventAsync(evt));
		}

		public void Dispose()
		{
			_scope.Dispose();
			_serviceProvider.Dispose();
		}

		[Theory]
		[InlineData("9e8d7c6b-5a4f-4e3d-2c1b-0a9f8e7d6c5b")]
		public async Task Get_BookingId_ReturnBooking(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var status = BookingStatus.Pending;

			var newBooking = await _bookingService.CreateBookingAsync(eventId);
			var booking = await _bookingService.GetBookingByIdAsync(newBooking.Id);

			Assert.True(booking.Id != Guid.Empty);
			Assert.True(booking.EventId == eventId);
			Assert.True(booking.Status == status);
		}

		[Fact]
		public async Task Get_BookingId_ReturnNoBooking()
		{
			var bookingId = Guid.NewGuid();

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.GetBookingByIdAsync(bookingId));

			Assert.Equal($"Не найдена бронь с таким ИД {bookingId}", exc?.Message);
		}
	}
}
