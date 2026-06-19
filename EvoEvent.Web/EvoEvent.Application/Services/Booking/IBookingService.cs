using EvoEvent.Domain.Entities;

namespace EvoEvent.Application.Services
{
	public interface IBookingService
	{
		Task<bool> CancelledBookingAsync(Guid id, CancellationToken token = default);

		Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken token = default);

		Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default);
	}
}
