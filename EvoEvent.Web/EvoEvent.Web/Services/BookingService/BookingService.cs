using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Services.BookingService
{
	public class BookingService : IBookingService
	{
		private readonly IEventService _eventService;
		private readonly static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private readonly IBookingRepository _bookingRepository;

		public BookingService(
			IEventService eventService,
			IBookingRepository bookingRepository
			)
		{
			_eventService = eventService;
			_bookingRepository = bookingRepository;
		}

		public async Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken token = default)
		{
			await _semaphore.WaitAsync(token);

			try
			{
				if (eventId == Guid.Empty)
					throw new ValidationException($"Передан не валидный параметр eventId = {eventId}");

				var eventExp = await _eventService.GetByIdAsync(eventId, token);

				if (eventExp is null)
					throw new NotFoundException($"Не найдено событие с таким ИД {eventId}");

				if (!eventExp.TryReserveSeats())
					throw new NoAvailableSeatsException("No available seats for this event");

				var newBooking = new Booking(eventId, BookingStatus.Pending, DateTime.UtcNow);
				await _bookingRepository.AddBookingAsync(newBooking, token);
				await _bookingRepository.SaveChangesAsync(token);

				return newBooking;
			}
			finally
			{
				_semaphore.Release();
			}
		}
				
		public async Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken token = default)
		{
			var booking = await _bookingRepository.GetBookingByIdAsync(bookingId, token);

			if (booking == null)
				throw new NotFoundException($"Не найдена бронь с таким ИД {bookingId}");

			return booking;
		}

	}
}
