using EvoEvent.Web.Models;

namespace EvoEvent.Web.Repositories
{
	public interface IBookingRepository
	{
		Task AddBookingAsync(Booking newBooking, CancellationToken token);
		Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken token);
		Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status);
		void RemoveBooking(Booking booking);
		Task SaveChangesAsync(CancellationToken token);
	}
}