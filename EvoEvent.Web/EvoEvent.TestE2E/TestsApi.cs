using EvoEvent.Infrastructure.Persistence.Repositories;
using EvoEvent.Presentation.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace EvoEvent.TestE2E
{
	public class TestsApi : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly WebApplicationFactory<Program> _factory;

		public TestsApi(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		#region login

		[Fact]
		public async Task POST_Login_Returns200Ok()
		{
			// Arrange
			var client = _factory.CreateClient();
			var request = new LoginRequestDto
			{
				Login = "denis",
				Password = "string123"
			};

			// Act
			var response = await client.PostAsJsonAsync("/auth/login", request);

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		#endregion

		#region Events

		[Fact]
		public async Task POST_Event_Returns201Created()
		{
			// Arrange
			var client = _factory.CreateClient();
			var requestEvent = new EventRequestDto
			{
				Title = "string1111",
				Description = "string1111",
				StartAt = DateTime.UtcNow,
				EndAt = DateTime.UtcNow.AddDays(2),
				TotalSeats = 15
			};
			var requestLogin = new LoginRequestDto
			{
				Login = "denis",
				Password = "string123"
			};			

			var responseUser = await client.PostAsJsonAsync("/auth/login", requestLogin);
			var loginResponse = await responseUser.Content.ReadFromJsonAsync<LoginResponseDto>();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResponse.Token}");

			// Act
			var responseEvent = await client.PostAsJsonAsync("/events", requestEvent);

			// Assert
			Assert.Equal(HttpStatusCode.Created, responseEvent.StatusCode);
		}

		#endregion

		#region Booking

		[Fact]
		public async Task POST_Booking_ReturnsAccepted()
		{
			// Arrange
			var client = _factory.CreateClient();
			var requestEvent = new EventRequestDto
			{
				Title = "string1111",
				Description = "string1111",
				StartAt = DateTime.UtcNow.AddDays(1),
				EndAt = DateTime.UtcNow.AddDays(2),
				TotalSeats = 15
			};
			var requestLogin = new LoginRequestDto
			{
				Login = "denis",
				Password = "string123"
			};

			var response= await client.PostAsJsonAsync("/auth/login", requestLogin);
			var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResponse.Token}");

			response = await client.PostAsJsonAsync("/events", requestEvent);
			var responseEvent = await response.Content.ReadFromJsonAsync<ResultResponse<EventResponseDto>>();

			// Act
			response = await client.PostAsJsonAsync($"/events/{responseEvent.Data.Id}/book", new { });

			// Assert
			Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
		}

		#endregion

	}
}
