using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Services.BookingService
{
	public class BookingService : IBookingService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private static readonly ConcurrentQueue<Booking> _queue = new();
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		public BookingService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		public async Task<Booking> CreateBookingAsync(Guid eventId)
		{
			if (eventId == Guid.Empty)
				throw new ValidationException($"Передан не валидный параметр eventId = {eventId}");

			using var scope = _scopeFactory.CreateScope();
			var eventService = scope.ServiceProvider.GetService<IEventService>();

			var eventExp = eventService.GetById(eventId);

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

			var newBooking = new Booking(Guid.NewGuid(), eventId, BookingStatus.Pending, DateTime.Now);

			_queue.Enqueue(newBooking);

			return newBooking;
		}

		public async Task<Booking> GetBookingByIdAsync(Guid bookingId)
		{
			var booking = _queue.FirstOrDefault(b => b.Id == bookingId);

			if (booking == null)
				throw new NotFoundException($"Не найдена бронь с таким ИД {bookingId}");

			return booking;
		}

		public bool TryBooking(out Booking booking)
		{
			booking = _queue.FirstOrDefault(b => b.Status == BookingStatus.Pending);

			return booking != null;
		}

		public IEnumerable<Booking> GetPending()
			=> _queue.Where(q => q.Status == BookingStatus.Pending);

		public void Update(Booking booking)
		{

		}

		public Booking Confirm(Booking booking)
		{
			booking.Status = BookingStatus.Confirmed;
			booking.ProcessedAt = DateTime.Now;

			return booking;
		}
		public Booking Reject(Booking booking)
		{
			booking.Status = BookingStatus.Rejected;
			booking.ProcessedAt = DateTime.Now;
		
			return booking;
		}
	}
}
