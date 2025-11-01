using System.Threading.Tasks;
using BackEndService.Core.Models.Payment;

namespace BackEndService.Core.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentCard> CreateCardAsync(string userId, string cardNumber, int expMonth, int expYear, string cvc);
        Task<PaymentCard?> GetCardAsync(string cardId, string userId);
        Task<bool> ChargeCardAsync(string cardId, string userId, decimal amount, string currency, string? apiKey);
        Task<PaymentResult> ChargeCardDirectAsync(string cardNumber, int expMonth, int expYear, string cvc, decimal amount, string currency, string userId, string? apiKey);
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

