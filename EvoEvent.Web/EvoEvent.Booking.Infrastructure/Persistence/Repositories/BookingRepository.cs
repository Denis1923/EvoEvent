using EvoEvent.Booking.Application.Abstractions;
using EvoEvent.Booking.Domain.Enums;
using EvoEvent.Booking.Infrastructure.Persistence.DataAccess;
using Microsoft.EntityFrameworkCore;
using BookingEnt = EvoEvent.Booking.Domain.Entities.Booking;

namespace EvoEvent.Booking.Infrastructure.Persistence.Repositories
{
	public class BookingRepository : IBookingRepository
	{
		private readonly AppDbContext _context;

		public BookingRepository(AppDbContext context)
		{
			_context = context;	
		}

		public async Task AddBookingAsync(BookingEnt newBooking, CancellationToken token = default)
			=> await _context.Bookings.AddAsync(newBooking, token);

		public async Task<BookingEnt?> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default)
			=> await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, token);

		public async Task<List<BookingEnt>> GetBookingsByEventIdAsync(Guid eventId, CancellationToken token = default)
			=> await _context.Bookings.Where(b => b.EventId == eventId).ToListAsync(token);

		public async Task<List<BookingEnt>> GetBookingsByStatusAsync(BookingStatus status, CancellationToken token = default)
			=> await _context.Bookings.Where(b => b.Status == status).ToListAsync(token);

		public async Task<List<BookingEnt>> GetConfirmedBookingUserAsync(Guid userId, CancellationToken token = default)
			=> await _context.Bookings.Where(b => b.UserId == userId && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending)).ToListAsync(token);

		public void RemoveBooking(BookingEnt booking)
			=> _context.Bookings.Remove(booking);

		public async Task SaveChangesAsync(CancellationToken token = default)
			=> await _context.SaveChangesAsync(token);
	}
}
