using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace NextJob.Tests
{
    public class BasicIntegrationTests : IClassFixture<TestApplicationFactory>
    {
        private readonly WebApplicationFactory<Program> _factory;

        [Fact]
        public async Task SwaggerEndpoint_IsReachable()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/swagger/v1/swagger.json");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task HealthEndpoint_Exists()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}



