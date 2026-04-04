using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;

namespace EvoEvent.Web.Services.BookingService
{
	public class BookingService : IBookingService
	{
		private readonly List<Booking> _bookings = new();

		public BookingService()
		{
			
		}

		public async Task<Guid> CreateBookingAsync(Guid eventId)
		{
			var newBooking = new Booking
			{
				EventId = eventId
			};

			_bookings.Add(newBooking);

			return newBooking.Id;
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
