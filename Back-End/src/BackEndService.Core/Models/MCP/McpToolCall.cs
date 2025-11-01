using System.Text.Json;

namespace BackEndService.Core.Models.MCP
{
    public class McpToolCall
    {
        public string Tool { get; set; } = string.Empty;
        public string? Workflow { get; set; }
        public JsonElement? Arguments { get; set; }
    }
}


