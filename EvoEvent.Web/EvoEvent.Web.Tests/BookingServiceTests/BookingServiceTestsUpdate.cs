using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsUpdate
	{
		private readonly IBookingService _bookingService;

		public BookingServiceTestsUpdate()
		{
			var mockScope = new Mock<IServiceScopeFactory>();
			_bookingService = new BookingService(mockScope.Object);
		}

		[Theory]
		[InlineData("7c9e6679-7425-40de-944b-e07fc1f90ae7")]
		public async void Update_BookingId_ReturnBooking(string bookingIdStr)
		{
			var bookingId = Guid.Parse(bookingIdStr);
			var status = BookingStatus.Confirmed;

			var booking = await _bookingService.GetBookingByIdAsync(bookingId);
			booking.Status = BookingStatus.Confirmed;
			booking = await _bookingService.GetBookingByIdAsync(bookingId);

			Assert.True(booking.Status == status);
		}
	}
}
