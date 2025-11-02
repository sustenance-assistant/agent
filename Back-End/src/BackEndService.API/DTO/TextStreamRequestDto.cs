using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackEndService.API.DTO
{
    public class TextStreamRequestDto
    {
        [Required]
        [MaxLength(4000)]
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}


