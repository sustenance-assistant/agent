using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;
using BackEndService.Core.Models.Payment;

namespace BackEndService.Workflows.Modules.Payment
{
    public class BillingWorkflowModule : IWorkflowModule
    {
        private readonly IPaymentService _paymentService;
        private readonly IContextStore _contextStore;

        public BillingWorkflowModule(IPaymentService paymentService, IContextStore contextStore)
        {
            _paymentService = paymentService;
            _contextStore = contextStore;
        }

        public async Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            // Retrieve user context for additional info
            var userContext = await _contextStore.GetAsync(context.SessionId ?? context.UserId);
            
            BillingRequest? billingRequest = null;
            if (input is JsonElement json)
            {
                billingRequest = JsonSerializer.Deserialize<BillingRequest>(json.GetRawText());
            }
            else if (input is BillingRequest req)
            {
                billingRequest = req;
            }

            if (billingRequest == null)
            {
                throw new System.ArgumentException("Invalid billing request");
            }

            // Inject context data
            billingRequest.UserId = context.UserId;
            if (userContext != null && !string.IsNullOrEmpty(userContext.PaymentId))
            {
                // Use stored payment ID if card ID not provided
                if (string.IsNullOrEmpty(billingRequest.CardId))
                {
                    billingRequest.CardId = userContext.PaymentId;
                }
            }

            // Charge using saved card OR full card details (Stripe headless style)
            bool success;
            string? transactionId = null;
            string? errorMessage = null;

            if (!string.IsNullOrEmpty(billingRequest.CardId))
            {
                // Use saved card
                success = await _paymentService.ChargeCardAsync(
                    billingRequest.CardId,
                    billingRequest.UserId,
                    billingRequest.TotalAmount,
                    billingRequest.Currency,
                    billingRequest.ApiKey
                );
                transactionId = success ? $"txn_{Guid.NewGuid():N}" : null;
            }
            else if (!string.IsNullOrEmpty(billingRequest.CardNumber) && 
                     billingRequest.ExpMonth.HasValue && 
                     billingRequest.ExpYear.HasValue && 
                     !string.IsNullOrEmpty(billingRequest.Cvc))
            {
                // Process full card details directly (Stripe headless checkout)
                var result = await _paymentService.ChargeCardDirectAsync(
                    billingRequest.CardNumber,
                    billingRequest.ExpMonth.Value,
                    billingRequest.ExpYear.Value,
                    billingRequest.Cvc,
                    billingRequest.TotalAmount,
                    billingRequest.Currency,
                    billingRequest.UserId,
                    billingRequest.ApiKey
                );
                success = result.Success;
                transactionId = result.TransactionId;
                errorMessage = result.ErrorMessage;
            }
            else
            {
                throw new ArgumentException("Either cardId or full card details (cardNumber, expMonth, expYear, cvc) must be provided");
            }

            return new 
            { 
                success, 
                transactionId,
                errorMessage,
                amount = billingRequest.TotalAmount, 
                currency = billingRequest.Currency 
            };
        }
    }
}

