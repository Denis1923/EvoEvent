using EvoEvent.Web.Models;
using EvoEvent.Web.Services.BookingService;

namespace EvoEvent.Web.Services
{
	public class BookingBackgroundService : BackgroundService
	{
		private readonly IBookingService _bookingService;
		private readonly ILogger<BookingBackgroundService> _logger;

		public BookingBackgroundService(
			IBookingService bookingService, 
			ILogger<BookingBackgroundService> logger)
		{
			_bookingService = bookingService;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Запущена фоновая обработка брони");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					bool isPending = _bookingService.TryBooking(out Booking booking)
									&& booking.Status == BookingStatus.Pending;

					_logger.LogInformation($"Запущена обработка брони.ИД = {booking.Id}");

					if (isPending)
					{
						await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
						booking.Status = BookingStatus.Confirmed;
						booking.ProcessedAt = DateTime.Now;	
					}
				}
				catch (OperationCanceledException)
				{
					_logger.LogError("Прервана фоновая обработка брони");
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError($"Заверна фоновый процесс с ошибкой: {ex.Message}");
				}
			}

			_logger.LogInformation("Завершена фоновая обработка брони");
		}
	}
}
