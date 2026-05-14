using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoEvent.Web.Repositories
{
	public class BookingRepository : IBookingRepository
	{
		private readonly AppDbContext _context;

		public BookingRepository(AppDbContext context)
		{
			_context = context;	
		}

		public async Task AddBookingAsync(Booking newBooking, CancellationToken token)
			=> await _context.Bookings.AddAsync(newBooking, token);

		public async Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken token)
			=> await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, token);

		public Booking? GetBookingsByStatus(BookingStatus status)
			=> _context.Bookings.FirstOrDefault(b => b.Status == status);

		public async Task SaveChangesAsync(CancellationToken token)
			=> await _context.SaveChangesAsync(token);
	}
}
