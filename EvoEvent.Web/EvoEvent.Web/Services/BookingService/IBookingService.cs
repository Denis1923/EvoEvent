using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services.BookingService
{
	public interface IBookingService
	{
		Task<Booking> CreateBookingAsync(Guid eventId);

		Task<Booking> GetBookingByIdAsync(Guid bookingId);
	}
}
