using BookingEnt = EvoEvent.Booking.Domain.Entities.Booking;
using EvoEvent.Booking.Domain.Enums;

namespace EvoEvent.Booking.Application.Abstractions.Repositories
{
	public interface IBookingRepository
	{
		Task AddBookingAsync(BookingEnt newBooking, CancellationToken token = default);
		Task<BookingEnt?> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default);
		Task<List<BookingEnt>> GetBookingsByEventIdAsync(Guid eventId, CancellationToken token = default);
		Task<List<BookingEnt>> GetBookingsByStatusAsync(BookingStatus status, CancellationToken token = default);
		Task<List<BookingEnt>> GetConfirmedBookingUserAsync(Guid userId, CancellationToken token = default);
		void RemoveBooking(BookingEnt booking);
		Task SaveChangesAsync(CancellationToken token = default);
	}
}