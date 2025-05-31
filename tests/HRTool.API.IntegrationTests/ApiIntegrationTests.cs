using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace HRTool.API.IntegrationTests
{
    // Custom factory to set required environment variables for integration tests
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Set required environment variables for the test host
            Environment.SetEnvironmentVariable("JWT_KEY", "TestSuperSecretKey1234567890123456_Extra"); // 32+ chars
            Environment.SetEnvironmentVariable("HRTOOL_ADMIN_PASSWORD", "TestAdminPassword123!");
            return base.CreateHost(builder);
        }
    }

    public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Auth_Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/auth/login", new { Email = "invalid@hrtool.local", Password = "WrongPassword!" });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Auth_Login_WithValidAdminCredentials_ReturnsToken()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/auth/login", new { Email = "admin@hrtool.local", Password = "TestAdminPassword123!" });
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            json.TryGetProperty("token", out var tokenProp).Should().BeTrue();
            tokenProp.GetString().Should().NotBeNullOrWhiteSpace();
        }

        // Add more integration tests for other endpoints as needed
    }
}
