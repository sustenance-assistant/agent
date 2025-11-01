namespace BackEndService.Core.Models.Payment
{
    public class PaymentCard
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Last4 { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public bool IsDefault { get; set; }
    }
}

