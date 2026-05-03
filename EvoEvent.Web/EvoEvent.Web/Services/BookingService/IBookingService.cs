using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services.BookingService
{
	public interface IBookingService
	{
		Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken token);

		Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken token);

		bool TryBooking(out Booking? booking);
	}
}
