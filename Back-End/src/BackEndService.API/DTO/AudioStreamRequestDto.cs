using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BackEndService.API.DTO
{
    public class AudioStreamRequestDto
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}


