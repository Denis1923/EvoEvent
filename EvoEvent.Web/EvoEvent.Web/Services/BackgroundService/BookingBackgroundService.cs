using EvoEvent.Web.Models;
using EvoEvent.Web.Services.BookingService;

namespace EvoEvent.Web.Services
{
	public class BookingBackgroundService : BackgroundService
	{
		private readonly IBookingService _bookingService;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<BookingBackgroundService> _logger;
		private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

		public BookingBackgroundService(
			IBookingService bookingService,
			IServiceScopeFactory scopeFactory,
			ILogger<BookingBackgroundService> logger)
		{
			_bookingService = bookingService;
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Запущена фоновая обработка брони");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var pendingBookings = _bookingService.GetPending().ToList();
					var tasks = pendingBookings.Select(booking => ProcessBookingAsync(booking, stoppingToken));
					await Task.WhenAll(tasks);
				}
				catch (OperationCanceledException)
				{
					_logger.LogError("Прервана фоновая обработка брони");
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError($"Завершен фоновый процесс с ошибкой: {ex.Message}");
				}
			}

			_logger.LogInformation("Завершена фоновая обработка брони");
		}

		private async Task ProcessBookingAsync(Booking booking, CancellationToken stoppingToken)
		{
			_logger.LogInformation($"Запущена обработка брони.ИД = {booking.Id}");

			await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

			using var scope = _scopeFactory.CreateScope();
			var eventService = scope.ServiceProvider.GetService<IEventService>();

			await _processingSemaphore.WaitAsync();

			var eventExp = default(Event);

			try
			{
				eventExp = eventService.GetById(booking.EventId);

				if (eventExp != null)
				{
					_bookingService.Confirm(booking);
				}
				else
				{
					_bookingService.Reject(booking);
					_logger.LogWarning($"Нет события.ИД ={booking.EventId} для брони.ИД = {booking.Id}, эту бронь отменяем");
				}
			}
			catch (OperationCanceledException)
			{
				_bookingService.Reject(booking);
				eventExp?.ReleaseSeats();
			}
			catch (Exception)
			{
				_bookingService.Reject(booking);
				eventExp?.ReleaseSeats();
			}
			finally
			{
				_processingSemaphore.Release();
			}
		}
	}
}
