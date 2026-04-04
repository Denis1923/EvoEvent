using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services.BookingService
{
	public interface IBookingService
	{
		Task<Guid> CreateBookingAsync(Guid eventId);

		Task<Booking> GetBookingByIdAsync(Guid bookingId);
	}
}
