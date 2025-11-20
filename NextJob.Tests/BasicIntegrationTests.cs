using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace NextJob.Tests
{
    public class BasicIntegrationTests : IClassFixture<TestApplicationFactory>
    {
        private readonly HttpClient _client;

        public BasicIntegrationTests(TestApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SwaggerJson_Endpoint_Returns_OK()
        {
            var response = await _client.GetAsync("/swagger/v1/swagger.json");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task HealthEndpoint_Exists_And_IsReachable()
        {
            var response = await _client.GetAsync("/health");

            // Pode ser 200 ou 503 dependendo do estado, mas não pode ser 404
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}






