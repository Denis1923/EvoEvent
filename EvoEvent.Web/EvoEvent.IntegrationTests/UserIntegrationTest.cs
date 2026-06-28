using EvoEvent.Application.DTOs;
using EvoEvent.Domain.Entities;
using EvoEvent.Domain.Enums;
using EvoEvent.Infrastructure.Persistence.DataAccess;
using EvoEvent.Infrastructure.Persistence.Repositories;
using EvoEvent.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EvoEvent.IntegrationTests
{
	public class UserIntegrationTest : IClassFixture<PostgreSqlFixture>
	{
		private readonly PostgreSqlContainer _postgres;
		private readonly IConfiguration _configuration;

		public UserIntegrationTest(PostgreSqlFixture fixture)
		{
			_postgres = fixture.Container;
			_configuration = new ConfigurationBuilder()
							   .SetBasePath(Directory.GetCurrentDirectory())
							   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
							   .AddEnvironmentVariables()
							   .Build();
		}

		private async Task<AppDbContext> CreateContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
										.UseNpgsql(_postgres.GetConnectionString())
										.Options;


			var context = new AppDbContext(options);
			await context.Database.MigrateAsync();
			return context;
		}

		private async Task ResetDataBaseAsync()
		{
			NpgsqlConnection.ClearAllPools();
			using var context = await CreateContext();
			await context.Database.EnsureDeletedAsync();
			await context.Database.MigrateAsync();
		}

		#region Register

		[Fact]
		public async Task Register_ReturnSuccess()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var userRepository = new UserRepository(context);
			var hashService = new HashService();

			var hashPassword = hashService.ConvertHashPassword("password1!");
			var newUser = new User(Guid.NewGuid(), "UserOne", hashPassword, Roles.User);

			// Act
			await userRepository.CreateUserAsync(newUser);
			await userRepository.SaveChangesAsync();

			// Assert
			await using var verifyContext = await CreateContext();
			userRepository = new UserRepository(context);
			var expUser = await userRepository.GetUserByIdAsync(newUser.UserId);

			Assert.NotNull(expUser);
			Assert.Equal(expUser.Login, newUser.Login);
		}

		#endregion

		#region login

		[Fact]
		public async Task Login_ReturnSuccess()
		{
			await ResetDataBaseAsync();

			// Arrange
			await using var context = await CreateContext();
			var userRepository = new UserRepository(context);
			var hashService = new HashService();
			var jwtService = new JwtService();
			var userService = new UserService(userRepository, hashService, jwtService, _configuration);

			var userDto = new UserDto
			{
				UserId = Guid.NewGuid(),
				Login = "UserTwo",
				Password = "password1!",
				Role = Roles.User
			};

			var hashPassword = hashService.ConvertHashPassword(userDto.Password);
			var newUser = new User(userDto.UserId, userDto.Login, hashPassword, userDto.Role);
			await userRepository.CreateUserAsync(newUser);
			await userRepository.SaveChangesAsync();

			// Act
			var token = await userService.LoginUserAsync(userDto);

			// Assert
			Assert.NotNull(token);
		}

		#endregion
	}
}
