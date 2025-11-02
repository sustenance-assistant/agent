using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Gateway.Services;
using Xunit;

namespace BackEndService.UnitTests
{
    public class MCPHandlerTests
    {
        [Fact]
        public async Task ListToolsAsync_ReturnsExpectedTools()
        {
            var handler = new MCPHandler();
            var request = JsonDocument.Parse("{}");
            
            var result = await handler.ListToolsAsync(request);
            var tools = result.RootElement.GetProperty("tools");
            
            Assert.True(tools.GetArrayLength() > 0);
            Assert.Contains("order", tools.EnumerateArray().Select(e => e.GetString()).Where(s => s != null)!);
        }

        [Fact]
        public async Task CallToolAsync_ReturnsStatus()
        {
            var handler = new MCPHandler();
            var request = JsonDocument.Parse("{\"tool\":\"test\"}");
            
            var result = await handler.CallToolAsync(request);
            
            Assert.NotNull(result);
        }
    }
}

