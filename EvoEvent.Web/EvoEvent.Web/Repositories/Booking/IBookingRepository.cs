using EvoEvent.Web.Models;

namespace EvoEvent.Web.Repositories
{
	public interface IBookingRepository
	{
		Task AddBookingAsync(Booking newBooking, CancellationToken token);
		Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken token);
		Booking? GetBookingsByStatus(BookingStatus pending);
		Task SaveChangesAsync(CancellationToken token);
	}
}