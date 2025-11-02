using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Models.Payment;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace BackEndService.Workflows.Services
{
    public class PaymentService : IPaymentService
    {
        private static readonly ConcurrentDictionary<string, PaymentCard> Cards = new();
        private readonly IConfiguration _configuration;
        private readonly string _stripeApiKey;

        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
            _stripeApiKey = _configuration["Services:Payment:ApiKey"] ?? "";
            
            // Initialize Stripe if API key is configured
            if (!string.IsNullOrEmpty(_stripeApiKey) && !_stripeApiKey.Contains("your-"))
            {
                StripeConfiguration.ApiKey = _stripeApiKey;
            }
        }

        public Task<PaymentCard> CreateCardAsync(string userId, string cardNumber, int expMonth, int expYear, string cvc)
        {
            var card = new PaymentCard
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Last4 = cardNumber.Length >= 4 ? cardNumber[^4..] : "****",
                Brand = DetectBrand(cardNumber),
                ExpMonth = expMonth,
                ExpYear = expYear,
                IsDefault = !Cards.Values.Any(c => c.UserId == userId)
            };

            Cards[card.Id] = card;
            return Task.FromResult(card);
        }

        public Task<PaymentCard?> GetCardAsync(string cardId, string userId)
        {
            if (Cards.TryGetValue(cardId, out var card) && card.UserId == userId)
            {
                return Task.FromResult<PaymentCard?>(card);
            }
            return Task.FromResult<PaymentCard?>(null);
        }

        public Task<bool> ChargeCardAsync(string cardId, string userId, decimal amount, string currency, string? apiKey)
        {
            // Validate card exists and belongs to user
            if (!Cards.TryGetValue(cardId, out var card) || card.UserId != userId)
            {
                return Task.FromResult(false);
            }

            // Simulate charge with saved card (in production, use Stripe Payment Methods API)
            return Task.FromResult(true);
        }

        public async Task<PaymentResult> ChargeCardDirectAsync(string cardNumber, int expMonth, int expYear, string cvc, decimal amount, string currency, string userId, string? apiKey)
        {
            // Validate card details
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 13 || cardNumber.Length > 19)
            {
                return new PaymentResult 
                { 
                    Success = false, 
                    ErrorMessage = "Invalid card number" 
                };
            }

            if (expYear < DateTime.Now.Year || (expYear == DateTime.Now.Year && expMonth < DateTime.Now.Month))
            {
                return new PaymentResult 
                { 
                    Success = false, 
                    ErrorMessage = "Card has expired" 
                };
            }

            // Use real Stripe API if configured, otherwise simulate
            if (string.IsNullOrEmpty(_stripeApiKey) || _stripeApiKey.Contains("your-"))
            {
                // Return simulated success
                var transactionId = $"txn_{Guid.NewGuid():N}";
                return new PaymentResult 
                { 
                    Success = true, 
                    TransactionId = transactionId 
                };
            }

            try
            {
                // Create Stripe Payment Intent
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100), // Stripe uses cents
                    Currency = currency.ToLower(),
                    PaymentMethodTypes = ["card"],
                    Metadata = new Dictionary<string, string>
                    {
                        { "user_id", userId }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                // In production, you'd use Stripe.js or Elements on frontend
                // For now, we simulate the charge
                // Note: In production, never send raw card details to your backend
                // Instead, use Stripe Elements/Payment Intents on frontend

                return new PaymentResult 
                { 
                    Success = true, 
                    TransactionId = paymentIntent.Id 
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResult 
                { 
                    Success = false, 
                    ErrorMessage = ex.Message 
                };
            }
        }

        private string DetectBrand(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber)) return "unknown";
            if (cardNumber.StartsWith("4")) return "visa";
            if (cardNumber.StartsWith("5")) return "mastercard";
            if (cardNumber.StartsWith("3")) return "amex";
            return "unknown";
        }
    }
}
