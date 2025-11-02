using System.ComponentModel.DataAnnotations;

namespace BackEndService.API.DTO.Payment
{
    /// <summary>
    /// Billing request that supports both saved card (cardId) or full card details (like Stripe headless)
    /// </summary>
    public class BillingRequestDto
    {
        /// <summary>
        /// Optional: Use saved card by ID. If not provided, card details must be supplied.
        /// </summary>
        public string? CardId { get; set; }

        /// <summary>
        /// Full card number (required if cardId not provided). Processed securely on backend like Stripe.
        /// </summary>
        [CreditCard]
        public string? CardNumber { get; set; }

        /// <summary>
        /// Expiration month (1-12). Required if cardNumber provided.
        /// </summary>
        [Range(1, 12)]
        public int? ExpMonth { get; set; }

        /// <summary>
        /// Expiration year. Required if cardNumber provided.
        /// </summary>
        [Range(2024, 2099)]
        public int? ExpYear { get; set; }

        /// <summary>
        /// CVC/CVV code. Required if cardNumber provided.
        /// </summary>
        [MinLength(3)]
        [MaxLength(4)]
        public string? Cvc { get; set; }

        /// <summary>
        /// Amount to charge (required).
        /// </summary>
        [Required]
        [Range(0.01, 10000)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Currency code (default: USD).
        /// </summary>
        public string Currency { get; set; } = "USD";
    }
}

