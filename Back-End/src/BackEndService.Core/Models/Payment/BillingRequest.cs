namespace BackEndService.Core.Models.Payment
{
    public class BillingRequest
    {
        public string UserId { get; set; } = string.Empty;
        
        // Option 1: Use saved card
        public string? CardId { get; set; }
        
        // Option 2: Full card details (like Stripe headless checkout)
        public string? CardNumber { get; set; }
        public int? ExpMonth { get; set; }
        public int? ExpYear { get; set; }
        public string? Cvc { get; set; }
        
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? ApiKey { get; set; }
        public string? OrderId { get; set; }
    }
}

