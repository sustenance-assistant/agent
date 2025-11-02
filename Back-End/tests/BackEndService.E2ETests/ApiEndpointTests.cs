using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace BackEndService.E2ETests
{
    public class ApiEndpointTests
    {
        private readonly HttpClient _client;

        public ApiEndpointTests()
        {
            // Note: In a real E2E test, you'd use WebApplicationFactory
            // For now, this demonstrates the test structure
            _client = new HttpClient { BaseAddress = new System.Uri("http://localhost:5050") };
        }

        [Fact(Skip = "Requires running server - skip in CI")]
        public async Task HealthEndpoint_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/health");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("ok", content);
        }

        [Fact(Skip = "Requires running server - skip in CI")]
        public async Task TextStreamEndpoint_AcceptsJson_ReturnsResponse()
        {
            var payload = new { text = "I'd like a pizza" };
            var response = await _client.PostAsJsonAsync("/api/stream/text", payload);
            
            // Note: This will fail if server isn't running - that's expected for E2E
            // In real setup, use WebApplicationFactory or skip if server unavailable
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                // Skip test if server not running
                return;
            }
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(Skip = "Requires running server - skip in CI")]
        public async Task MCPListTools_ReturnsTools()
        {
            var response = await _client.PostAsync("/api/mcp/tools/list", new StringContent("", Encoding.UTF8, "application/json"));
            
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                return;
            }
            
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            Assert.True(json.RootElement.TryGetProperty("tools", out var tools));
        }
    }
}

