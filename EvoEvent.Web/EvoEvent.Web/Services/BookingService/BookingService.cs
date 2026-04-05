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

		public BookingService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
			var bookings = new List<Booking>()
			{
				new Booking(Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), BookingStatus.Pending, DateTime.Now),
				new Booking(Guid.Parse("6c9e6679-7425-40de-944b-e07fc1f90ae7"), Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), BookingStatus.Pending, DateTime.Now),
				new Booking(Guid.Parse("4c9e6679-7425-40de-944b-e07fc1f90ae7"), Guid.Parse("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b"), BookingStatus.Pending, DateTime.Now),
				new Booking(Guid.Parse("9c9e6679-7425-40de-944b-e07fc1f90ae7"), Guid.Parse("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b"), BookingStatus.Pending, DateTime.Now),
				new Booking(Guid.Parse("2c9e6679-7425-40de-944b-e07fc1f90ae7"), Guid.Parse("9e8d7c6b-5a4f-4e3d-2c1b-0a9f8e7d6c5b"), BookingStatus.Pending, DateTime.Now),
				new Booking(Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7"), Guid.Parse("123e4567-e89b-12d3-a456-426614174000"), BookingStatus.Pending, DateTime.Now)
			};

			foreach (var booking in bookings)
			{
				_queue.Enqueue(booking);
			}
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
	}
}
