using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EvoEvent.IntegrationTests
{
	internal class BookingIntegrationTest : IAsyncLifetime
	{
		private readonly PostgreSqlContainer _conteiner = new PostgreSqlBuilder().Build();

		public async Task DisposeAsync()
		{	
			await _conteiner.DisposeAsync();
		}

		public async Task InitializeAsync()
		{
			await _conteiner.StartAsync();
		}

		private AppDbContext CreateContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
								.UseNpgsql(_conteiner.GetConnectionString())
								.Options;


			var context = new AppDbContext(options);
			context.Database.Migrate();
			return context;
		}

		private async Task ResetDataBaseAsync()
		{
			NpgsqlConnection.ClearAllPools();
			using var context = CreateContext();
			await context.Database.EnsureDeletedAsync();
			await context.Database.MigrateAsync();	
		}

		#region Create

		public async Task CreateBooking_SavesBookingToDatabase()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = CreateContext();
			var eventRepository = new EventRepository(context);

			// Act

			// Assert
			await using var verifyContext = CreateContext();

		}

		#endregion

		#region Read

		public async Task GetBooking_BookingId_ReturnsBookingFromDatabase()
		{

		}

		#endregion

		#region Update

		public async Task UpdateBooking_StatusReject_UpdateBookingToDatabase()
		{

		}

		#endregion

	}
}
