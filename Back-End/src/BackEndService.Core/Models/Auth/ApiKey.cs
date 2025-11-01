namespace BackEndService.Core.Models.Auth
{
    public class ApiKey
    {
        public string Key { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

