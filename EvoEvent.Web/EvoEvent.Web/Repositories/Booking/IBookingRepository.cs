using EvoEvent.Web.Models;

namespace EvoEvent.Web.Repositories
{
	public interface IBookingRepository
	{
		Task AddBookingAsync(Booking newBooking, CancellationToken token = default);
		Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default);
		Task<List<Booking>> GetBookingsByEventIdAsync(Guid eventId, CancellationToken token = default);
		Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status, CancellationToken token = default);
		void RemoveBooking(Booking booking);
		Task SaveChangesAsync(CancellationToken token = default);
	}
}