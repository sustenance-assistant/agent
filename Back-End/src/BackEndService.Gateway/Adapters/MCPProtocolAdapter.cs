using System.Text.Json;

namespace BackEndService.Gateway.Adapters
{
    public class MCPProtocolAdapter
    {
        public JsonDocument Adapt(JsonDocument request) => request;
    }
}


