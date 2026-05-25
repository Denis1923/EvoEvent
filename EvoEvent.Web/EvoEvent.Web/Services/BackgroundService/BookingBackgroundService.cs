using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;

namespace EvoEvent.Web.Services
{
	public class BookingBackgroundService : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<BookingBackgroundService> _logger;
		private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

		public BookingBackgroundService(
			IServiceScopeFactory scopeFactory,
			ILogger<BookingBackgroundService> logger)
		{
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
					using var scope = _scopeFactory.CreateScope();
					var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
					var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

					var pendingBookings = await bookingRepository.GetBookingsByStatusAsync(BookingStatus.Pending);

					var tasks = pendingBookings.Select(booking => 
												ProcessBookingAsync(booking.Id, stoppingToken));
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

		private async Task ProcessBookingAsync(Guid bookingId, CancellationToken stoppingToken)
		{
			_logger.LogInformation($"Запущена обработка брони.ИД = {bookingId}");

			await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

			using var scope = _scopeFactory.CreateScope();
			var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
			var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

			await _processingSemaphore.WaitAsync();

			var eventExp = default(Event);

			var booking = await bookingRepository.GetBookingByIdAsync(bookingId, stoppingToken);

			if (booking == null || booking.Status != BookingStatus.Pending)
				return;

			try
			{
				eventExp = await eventRepository.GetEventByIdAsync(booking.EventId, stoppingToken);

				if (eventExp is null)
				{
					booking.Reject();
					_logger.LogWarning($"Нет события.ИД ={booking.EventId} для брони.ИД = {booking.Id}, эту бронь отменяем");
				}
				else
					booking.Confirm();
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception)
			{
				booking.Reject();
				eventExp?.ReleaseSeats();
			}
			finally
			{
				await bookingRepository.SaveChangesAsync();
				_processingSemaphore.Release();
			}
		}
	}
}
