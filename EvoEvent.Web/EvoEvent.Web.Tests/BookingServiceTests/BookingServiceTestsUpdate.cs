using EvoEvent.Application.Abstractions;
using EvoEvent.Application.Abstractions.Repositories;
using EvoEvent.Application.Services;
using EvoEvent.Domain.Enums;
using EvoEvent.Infrastructure.Persistence.DataAccess;
using EvoEvent.Infrastructure.Persistence.Repositories;
using EvoEvent.Infrastructure.Services;
using EvoEvent.Web.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EvoEvent.Web.Tests.BookingServiceTests
{
	public class BookingServiceTestsUpdate : IDisposable
	{
		private readonly ServiceProvider _serviceProvider;
		private readonly IServiceScope _scope;
		private readonly IEventService _eventService;
		private readonly IBookingService _bookingService;
		private readonly IUserService _userService;

		public BookingServiceTestsUpdate()
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
		[InlineData("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d")]
		public async Task Confirm_BookingId_ReturnBooking(string eventIdstr)
		{
			var eventId = Guid.Parse(eventIdstr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");
			var status = BookingStatus.Confirmed;

			var newBooking = await _bookingService.CreateBookingAsync(eventId, userId);
			var booking = await _bookingService.GetBookingByIdAsync(newBooking.Id);
			booking.Confirm();

			Assert.True(booking.Status == status);
			Assert.True(booking.ProcessedAt.HasValue);
		}

		[Theory]
		[InlineData("1e2f3a4b-5c6d-4e7f-8a9b-0c1d2e3f4a5b")]
		public async Task Reject_BookingId_ReturnBooking(string eventIdstr)
		{
			var eventId = Guid.Parse(eventIdstr);
			var userId = Guid.Parse("347ac10b-58cc-4372-a567-0e02b2c3d479");
			var statusR = BookingStatus.Rejected;
			var statusP = BookingStatus.Pending;

			var evetnExp = await _eventService.GetByIdAsync(eventId);
			var newBooking = await _bookingService.CreateBookingAsync(eventId, userId);
			newBooking.Reject();
			var newBooking2 = await _bookingService.CreateBookingAsync(eventId, userId);

			Assert.True(newBooking.Status == statusR);
			Assert.True(newBooking2.Status == statusP);

		}
	}
}
