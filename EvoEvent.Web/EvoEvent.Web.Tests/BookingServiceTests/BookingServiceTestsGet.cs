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

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsGet : IDisposable
	{

		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IBookingService _bookingService;
		private readonly IEventService _eventService;
		private readonly IUserService _userService;

		public BookingServiceTestsGet()
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
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IHashService, HashService>();
			services.AddScoped<IJwtService,  JwtService>();

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
		[InlineData("9e8d7c6b-5a4f-4e3d-2c1b-0a9f8e7d6c5b")]
		public async Task Get_BookingId_ReturnBooking(string eventIdStr)
		{
			var eventId = Guid.Parse(eventIdStr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");
			var status = BookingStatus.Pending;

			var newBooking = await _bookingService.CreateBookingAsync(eventId, userId);
			var booking = await _bookingService.GetBookingByIdAsync(newBooking.Id);

			Assert.True(booking.Id != Guid.Empty);
			Assert.True(booking.EventId == eventId);
			Assert.True(booking.Status == status);
		}

		[Fact]
		public async Task Get_BookingId_ReturnNoBooking()
		{
			var bookingId = Guid.NewGuid();

			var exc = await Assert.ThrowsAsync<NotFoundException>(
				async () => await _bookingService.GetBookingByIdAsync(bookingId));

			Assert.Equal($"Не найдена бронь с таким ИД {bookingId}", exc?.Message);
		}
	}
}
