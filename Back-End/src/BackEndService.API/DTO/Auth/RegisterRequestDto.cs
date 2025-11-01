using System.ComponentModel.DataAnnotations;

namespace BackEndService.API.DTO.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        public string? Name { get; set; }
    }
}

