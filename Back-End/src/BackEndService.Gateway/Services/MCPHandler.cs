using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Gateway;

namespace BackEndService.Gateway.Services
{
    public class MCPHandler : IMCPHandler
    {
        public Task<JsonDocument> ListToolsAsync(JsonDocument request)
        {
            var json = JsonDocument.Parse("{\"tools\":[\"search\",\"order\",\"stt\",\"tts\",\"rag\"]}");
            return Task.FromResult(json);
        }

        public Task<JsonDocument> CallToolAsync(JsonDocument request)
        {
            var json = JsonDocument.Parse("{\"status\":\"ok\"}");
            return Task.FromResult(json);
        }
    }
}


