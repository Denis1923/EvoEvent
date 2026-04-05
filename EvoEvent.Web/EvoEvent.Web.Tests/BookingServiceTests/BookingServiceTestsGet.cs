using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsGet
	{
		private readonly IBookingService _bookingService;

		public BookingServiceTestsGet()
		{
			var mockScope = new Mock<IServiceScopeFactory>();
			_bookingService = new BookingService(mockScope.Object);
		}

		[Theory]
		[InlineData("4c9e6679-7425-40de-944b-e07fc1f90ae7", "a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b")]
		public async void Get_BookingId_ReturnBooking(string bookingIdStr, string eventIdStr)
		{
			var bookingId = Guid.Parse(bookingIdStr);
			var eventId = Guid.Parse(eventIdStr);
			var status = BookingStatus.Pending;

			var booking = await _bookingService.GetBookingByIdAsync(bookingId);

			Assert.True(booking.Id != Guid.Empty);
			Assert.True(booking.EventId == eventId);
			Assert.True(booking.Status == status);
		}

		[Fact]
		public async void Get_BookingId_ReturnNoBooking()
		{
			var bookingId = Guid.NewGuid();

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.GetBookingByIdAsync(bookingId));

			Assert.Equal($"Не найдена бронь с таким ИД {bookingId}", exc?.Message);
		}
	}
}
