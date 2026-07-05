using BookingEnt = EvoEvent.Booking.Domain.Entities.Booking;

namespace EvoEvent.Booking.Application.Services
{
	public interface IBookingService
	{
		Task<bool> CancelledBookingAsync(Guid id, Guid userId, CancellationToken token = default);

		Task<BookingEnt> CreateBookingAsync(Guid eventId, Guid userId, CancellationToken token = default);

		Task<BookingEnt> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default);

		Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
	}
}
