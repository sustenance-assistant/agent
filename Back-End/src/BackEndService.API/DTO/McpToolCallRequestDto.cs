using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackEndService.API.DTO
{
    public class McpToolCallRequestDto
    {
        [Required]
        [MaxLength(128)]
        [JsonPropertyName("tool")]
        public string Tool { get; set; } = string.Empty;

        [JsonPropertyName("workflow")]
        public string? Workflow { get; set; }

        [JsonPropertyName("arguments")]
        public JsonElement? Arguments { get; set; }
    }
}


