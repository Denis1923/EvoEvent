using Testcontainers.PostgreSql;

namespace EvoEvent.IntegrationTests
{
	public class PostgreSqlFixture : IAsyncLifetime
	{
		public PostgreSqlContainer Container { get; private set; }

		public async Task InitializeAsync()
		{
			Container = new PostgreSqlBuilder()
				.WithImage("postgres:16-alpine")
				.WithDatabase("evoevent")
				.Build();

			await Container.StartAsync();
		}

		public async Task DisposeAsync()
		{
			if (Container != null)
				await Container.DisposeAsync();
		}
	}
}
