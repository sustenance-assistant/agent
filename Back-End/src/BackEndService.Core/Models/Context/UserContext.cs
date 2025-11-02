namespace BackEndService.Core.Models.Context
{
    public class UserContext
    {
        public string UserId { get; set; } = string.Empty;
        public string? PaymentId { get; set; }
        public string? Location { get; set; }
    }
}


