using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Services.BookingService
{
	public class BookingService : IBookingService
	{
		private readonly static List<Booking> _bookings = new();

		public BookingService()
		{
			
		}

		public async Task<Booking> CreateBookingAsync(Guid eventId)
		{
			if (eventId == Guid.Empty)
				throw new ValidationException($"Передан не валидный параметр eventId = {eventId}");

			var newBooking = new Booking
			{
				EventId = eventId
			};

			_bookings.Add(newBooking);

			return newBooking;
		}

		public async Task<Booking> GetBookingByIdAsync(Guid bookingId)
		{
			var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);

			if (booking == null)
				throw new NotFoundException($"Не найдена бронь с таким ИД {bookingId}");

			return booking;
		}
		
	}
}
