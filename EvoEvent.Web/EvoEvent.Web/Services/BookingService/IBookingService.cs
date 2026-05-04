using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services.BookingService
{
	public interface IBookingService
	{
		Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken token = default);

		Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default);

		bool TryBooking(out Booking? booking);
	}
}
