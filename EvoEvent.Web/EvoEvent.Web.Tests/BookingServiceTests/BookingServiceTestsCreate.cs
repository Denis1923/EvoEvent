using EvoEvent.Application.Abstractions;
using EvoEvent.Application.Abstractions.Repositories;
using EvoEvent.Application.Services;
using EvoEvent.Domain.Enums;
using EvoEvent.Domain.Exceptions;
using EvoEvent.Infrastructure.Persistence.DataAccess;
using EvoEvent.Infrastructure.Persistence.Repositories;
using EvoEvent.Infrastructure.Services;
using EvoEvent.Web.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsCreate : IDisposable
	{
		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IEventService _eventService;
		private readonly IBookingService _bookingService;
		private readonly IUserService _userService;

		public BookingServiceTestsCreate()
		{
			var dbName = Guid.NewGuid().ToString();
			var services = new ServiceCollection();
			services.AddDbContext<AppDbContext>(options =>
				options.UseInMemoryDatabase(dbName));
			services.AddScoped<IEventService, EventService>();
			services.AddScoped<IBookingService, BookingService>();
			services.AddScoped<IEventRepository, EventRepository>();
			services.AddScoped<IBookingRepository, BookingRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IHashService, HashService>();
			services.AddScoped<IJwtService, JwtService>();

			var configuration = new ConfigurationBuilder()
			   .AddInMemoryCollection(new Dictionary<string, string>())
			   .Build();
			services.AddSingleton<IConfiguration>(configuration);

			_serviceProvider = services.BuildServiceProvider();
			_scope = _serviceProvider.CreateScope();
			_bookingService = _scope.ServiceProvider.GetRequiredService<IBookingService>();
			_eventService = _scope.ServiceProvider.GetRequiredService<IEventService>();
			_userService = _scope.ServiceProvider.GetRequiredService<IUserService>();

			var events = ModelEventServiceTests.GetEvents();
			events.ForEach(evt => _eventService.AddEventAsync(evt));

			var users = ModelUserServiceTest.GetUsers();
			users.ForEach(user => _userService.RegisterUserAsync(user));
		}

		public void Dispose()
		{
			_scope.Dispose();
			_serviceProvider.Dispose();
		}

		[Theory]
		[InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
		public async Task CreateBookingByEventId_ReturnIsStatusPending(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");

			var eventExp = await _eventService.GetByIdAsync(eventId);
			var newBooking = await _bookingService.CreateBookingAsync(eventId, userId);

			Assert.NotNull(newBooking);
			Assert.True(newBooking.Status == BookingStatus.Pending);
			Assert.Equal(eventExp.AvailableSeats, eventExp.TotalSeats - 1);
		}

		[Theory]
		[InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
		public async Task CreateBookingsByEventId_ReturnIsSuccess(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");
			var idsNewBooking = new List<Guid>();

			var eventExp = await _eventService.GetByIdAsync(eventId);

			for (int i = 0; i < eventExp.TotalSeats; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId, userId);
				idsNewBooking.Add(newBooking.Id);
			}

			Assert.Equal(eventExp.TotalSeats, idsNewBooking.Distinct().Count());
			Assert.Equal(eventExp.AvailableSeats, 0);
		}

		[Theory]
		[InlineData("123e4567-e89b-12d3-a456-426614174000")]
		public async Task CreateBookingsByEventId_ReturnNoAvailableSeats(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");
			var idsNewBooking = new List<Guid>();

			var eventExp = await _eventService.GetByIdAsync(eventId);

			for (int i = 0; i < eventExp.TotalSeats; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId, userId);
				idsNewBooking.Add(newBooking.Id);
			}

			var exc = await Assert.ThrowsAsync<NoAvailableSeatsException>(
				async () => await _bookingService.CreateBookingAsync(eventId, userId));

			Assert.Equal($"No available seats for this event", exc?.Message);
			Assert.Equal(eventExp.TotalSeats, idsNewBooking.Distinct().Count());
		}

		[Fact]
		public async Task Add_NewBooking_ReturnValidationException()
		{
			var eventId = Guid.Empty;
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");

			var exc = await Assert.ThrowsAsync<ValidationException>(
				async () => await _bookingService.CreateBookingAsync(eventId, userId));

			Assert.Equal($"Передан не валидный параметр eventId = {eventId}", exc?.Message);
		}

		[Theory]
		[InlineData("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a9b")]
		public async Task Add_NewBooking_ReturnNotFoundEvent(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.CreateBookingAsync(eventId, userId));

			Assert.Equal($"Не найдено событие с таким ИД {eventId}", exc?.Message);
		}

		[Theory]
		[InlineData("4f5e6d7c-8b9a-4e0f-1d2c-3a4b5c6d7e82")]
		public async Task Add_NewBooking_ReturnNotFoundDeleteEvent(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.CreateBookingAsync(eventId, userId));

			Assert.Equal($"Не найдено событие с таким ИД {eventId}", exc?.Message);
		}

		[Theory]
		[InlineData("8d8e9f0a-1b2c-4d3e-5f6a-7b8c9d0e1f2a")]
		public async Task Add_NewBooking_ReturnNoAvailableSeats(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");

			var exc = await Assert.ThrowsAsync<NoAvailableSeatsException>(
				async () => await _bookingService.CreateBookingAsync(eventId, userId));

			Assert.Equal($"No available seats for this event", exc?.Message);
		}

		[Theory]
		[InlineData("7c9e6679-7425-40de-944b-e07fc1f90ae7")]
		public async Task AddParralelBooking_ReturnBookings(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");
			var idsNewBooking = new List<Guid>();
			var results = new ConcurrentBag<(bool Success, NoAvailableSeatsException Exception)>();

			var eventExp = await _eventService.GetByIdAsync(eventId);

			var options = new ParallelOptions
			{
				MaxDegreeOfParallelism = 20
			};

			await Parallel.ForEachAsync(Enumerable.Range(0, 20), options, async (_, _) =>
			{
				try
				{
					using var scope = _serviceProvider.CreateScope();
					var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
					await bookingService.CreateBookingAsync(eventExp.Id, userId);

					results.Add((true, null));
				}
				catch (NoAvailableSeatsException ex)
				{
					results.Add((false, ex));
				}
			});

			// Assert
			var successFullCount = results.Count(r => r.Success);
			var noSeatsCount = results.Count(r => r.Exception is NoAvailableSeatsException);
			eventExp = await _eventService.GetByIdAsync(eventId);

			Assert.Equal(5, successFullCount);
			Assert.Equal(15, noSeatsCount);
		}

		[Theory]
		[InlineData("9a8b7c6d-5e4f-4a3b-2c1d-0e9f8a7b6c5d")]
		public async Task AddParralelBooking_ReturnDistinctBookings(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");
			var idsNewBooking = new List<Guid>();
			var results = new ConcurrentBag<Guid>();

			var eventExp = await _eventService.GetByIdAsync(eventId);

			var options = new ParallelOptions
			{
				MaxDegreeOfParallelism = 10
			};

			await Parallel.ForEachAsync(Enumerable.Range(0, 10), options, async (_, _) =>
			{
				using var scope = _serviceProvider.CreateScope();
				var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
				var newBooking = await bookingService.CreateBookingAsync(eventId, userId);
				results.Add(newBooking.Id);
			});

			// Assert
			var distincCount = results.Distinct().Count();

			Assert.Equal(10, distincCount);
		}

		[Theory]
		[InlineData("8c9e6679-7425-40de-944b-e07fc1f90ae8")]
		public async Task CreateBookingsByEventId_ReturnBookingPastEvent(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");

			var eventExp = await _eventService.GetByIdAsync(eventId);

			var exc = await Assert.ThrowsAsync<BookingPastEventException>(
				async () => await _bookingService.CreateBookingAsync(eventId, userId));

			Assert.Equal($"Событие уже началось, бронирование запрещено", exc?.Message);
		}

		[Theory]
		[InlineData("b1c4a9e3-7d2f-4a6e-8b5c-9e2d1f3a4b6c")]
		public async Task CreateBookingsByEventId_ReturnExceedingActiveBookingLimit(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");
			var idsNewBooking = new List<Guid>();
			var limitBooking = 10;

			var eventExp = await _eventService.GetByIdAsync(eventId);

			for (int i = 0; i < limitBooking; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId, userId);
				idsNewBooking.Add(newBooking.Id);
			}

			var exc = await Assert.ThrowsAsync<ExceedingActiveBookingLimitException>(
				async () => await _bookingService.CreateBookingAsync(eventId, userId));

			Assert.Equal($"Бронирование события запрещено, так как превышен лимит бронирования", exc?.Message);
		}

		[Theory]
		[InlineData("b1c4a9e3-7d2f-4a6e-8b5c-9e2d1f3a4b6c")]
		public async Task CreateBookingsByEventId_ReturnSuccessBookingDiffUsers(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userIdOne = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");
			var userIdTwo = Guid.Parse("417dc10b-58cc-5172-a567-0e02b2c3d489");
			var idsNewBooking = new List<Guid>();
			var idsNewBookingOneUser = new List<Guid>();
			var idsNewBookingTwoUser = new List<Guid>();
			var limitBookingForOneUser = 6;
			var limitBookingForTwoUser = 8;
			var allCountNewBooking = 14;

			var eventExp = await _eventService.GetByIdAsync(eventId);

			for (int i = 0; i < limitBookingForOneUser; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId, userIdOne);
				idsNewBookingOneUser.Add(newBooking.Id);
				idsNewBooking.Add(newBooking.Id);
			}

			for (int i = 0; i < limitBookingForTwoUser; i++)
			{
				var newBooking = await _bookingService.CreateBookingAsync(eventId, userIdTwo);
				idsNewBookingTwoUser.Add(newBooking.Id);
				idsNewBooking.Add(newBooking.Id);
			}

			Assert.Equal(idsNewBookingOneUser.Count, limitBookingForOneUser);
			Assert.Equal(idsNewBookingTwoUser.Count, limitBookingForTwoUser);
			Assert.Equal(idsNewBooking.Count, allCountNewBooking);
		}
	}
}
