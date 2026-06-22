using EvoEvent.Application.Abstractions;
using EvoEvent.Domain.Exceptions;
using EvoEvent.Domain.Entities;
using EvoEvent.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Application.Services
{
	public class BookingService : IBookingService
	{
		private readonly IEventService _eventService;
		private readonly static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private readonly IBookingRepository _bookingRepository;
		private readonly int _limitBookingCount = 10;

		public BookingService(
			IEventService eventService,
			IBookingRepository bookingRepository
			)
		{
			_eventService = eventService;
			_bookingRepository = bookingRepository;
		}

		public async Task<Booking> CreateBookingAsync(Guid eventId, Guid userId, CancellationToken token = default)
		{
			await _semaphore.WaitAsync(token);

			try
			{
				if (eventId == Guid.Empty)
					throw new ValidationException($"Передан не валидный параметр eventId = {eventId}");

				var eventExp = await _eventService.GetByIdAsync(eventId, token);

				if (eventExp is null)
					throw new NotFoundException($"Не найдено событие с таким ИД {eventId}");

				var nowDate = DateTime.UtcNow;
				var checkBookingDate = eventExp.StartAt.Date > nowDate && nowDate < eventExp.EndAt.Date;

				if (!checkBookingDate)
					throw new ValidationException("Событие уже началось, бронирование запрещено");

				var bookingsUser = await _bookingRepository.GetBookingUserByEventIdAsync(userId, eventId, token);

				if (bookingsUser.Count >= _limitBookingCount)
					throw new NoAvailableSeatsException("Бронирование события запрещено, так как превышен лимит бронирования");

				if (!eventExp.TryReserveSeats())
					throw new NoAvailableSeatsException("No available seats for this event");

				var newBooking = new Booking(eventId, BookingStatus.Pending, DateTime.UtcNow, Guid.NewGuid());
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

		public async Task<bool> CancelledBookingAsync(Guid id, CancellationToken token = default)
		{
			var booking = await GetBookingByIdAsync(id, token);
			var statusesCancelled = new BookingStatus[] { BookingStatus.Rejected, BookingStatus.Cancelled };

			if (booking is null || statusesCancelled.Contains(booking.Status))
				return false;

			booking.Cancelled();
			await _bookingRepository.SaveChangesAsync(token);

			return true;
		}
	}
}
