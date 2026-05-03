using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Services.BookingService
{
	public class BookingService : IBookingService
	{
		private readonly IEventService _eventService;
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private readonly AppDbContext _context;

		public BookingService(
			IEventService eventService,
			AppDbContext context
			)
		{
			_eventService = eventService;
			_context = context;
		}

		public async Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken token = default)
		{
			if (eventId == Guid.Empty)
				throw new ValidationException($"Передан не валидный параметр eventId = {eventId}");

			var eventExp = await _eventService.GetByIdAsync(eventId, token);

			if (eventExp is null)
				throw new NotFoundException($"Не найдено событие с таким ИД {eventId}");

			await _semaphore.WaitAsync();

			try
			{
				if (!eventExp.TryReserveSeats())
					throw new NoAvailableSeatsException("No available seats for this event");
			}
			finally
			{
				_semaphore.Release();
			}

			var newBooking = new Booking(Guid.NewGuid(), eventId, BookingStatus.Pending, DateTime.Now.ToUniversalTime());
			await _context.Bookings.AddAsync(newBooking, token);
			await _context.SaveChangesAsync(token);

			return newBooking;
		}
				
		public async Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default)
		{
			var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

			if (booking == null)
				throw new NotFoundException($"Не найдена бронь с таким ИД {bookingId}");

			return booking;
		}

		public bool TryBooking(out Booking? booking)
		{
			booking = _context.Bookings.FirstOrDefault(b => b.Status == BookingStatus.Pending);

			return booking != null;
		}

	}
}
