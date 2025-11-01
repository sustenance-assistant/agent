using System.ComponentModel.DataAnnotations;

namespace BackEndService.API.DTO.Payment
{
    public class CreateCardRequestDto
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [Range(1, 12)]
        public int ExpMonth { get; set; }

        [Required]
        [Range(2024, 2099)]
        public int ExpYear { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(4)]
        public string Cvc { get; set; } = string.Empty;
    }
}

