using EvoEvent.Domain.Entities;

namespace EvoEvent.Application.Services
{
	public interface IBookingService
	{
		Task<bool> CancelledBookingAsync(Guid id, Guid userId, CancellationToken token = default);

		Task<bool> CancelledBookingForAdminAsync(Guid id, CancellationToken token = default);

		Task<Booking> CreateBookingAsync(Guid eventId, Guid userId, CancellationToken token = default);

		Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default);
	}
}
