using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services.BookingService
{
	public interface IBookingService
	{
		Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken token);

		Task<Booking> GetBookingByIdAsync(Guid bookingId);

		bool TryBooking(out Booking booking);

		IEnumerable<Booking> GetPending();

		Booking Confirm(Booking booking);

		Booking Reject(Booking booking);
	}
}
