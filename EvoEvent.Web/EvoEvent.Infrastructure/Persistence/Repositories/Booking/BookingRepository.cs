using EvoEvent.Application.Abstractions;
using EvoEvent.Domain.Entities;
using EvoEvent.Domain.Enums;
using EvoEvent.Infrastructure.Persistence.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EvoEvent.Infrastructure.Persistence.Repositories
{
	public class BookingRepository : IBookingRepository
	{
		private readonly AppDbContext _context;

		public BookingRepository(AppDbContext context)
		{
			_context = context;	
		}

		public async Task AddBookingAsync(Booking newBooking, CancellationToken token = default)
			=> await _context.Bookings.AddAsync(newBooking, token);

		public async Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default)
			=> await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, token);

		public async Task<List<Booking>> GetBookingsByEventIdAsync(Guid eventId, CancellationToken token = default)
			=> await _context.Bookings.Where(b => b.EventId == eventId).ToListAsync(token);

		public async Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status, CancellationToken token = default)
			=> await _context.Bookings.Where(b => b.Status == status).ToListAsync(token);

		public void RemoveBooking(Booking booking)
			=> _context.Bookings.Remove(booking);

		public async Task SaveChangesAsync(CancellationToken token = default)
			=> await _context.SaveChangesAsync(token);
	}
}
