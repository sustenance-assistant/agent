using System.Threading.Tasks;
using BackEndService.API.DTO.Payment;
using BackEndService.API.Swagger;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;
using BackEndService.Core.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace BackEndService.API.Gateway.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IWorkflowOrchestrator _orchestrator;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, IWorkflowOrchestrator orchestrator, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _orchestrator = orchestrator;
            _configuration = configuration;
        }

        [HttpPost("cards")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Add payment card", Description = "Registers a new payment card for the user")]
        [SwaggerResponse(200, "Card created")]
        public async Task<IActionResult> CreateCard(
            [FromBody] CreateCardRequestDto request,
            [FromHeader(Name = "x-user-id")] string userId)
        {
            var card = await _paymentService.CreateCardAsync(
                userId, request.CardNumber, request.ExpMonth, request.ExpYear, request.Cvc);
            return Ok(new { cardId = card.Id, last4 = card.Last4, brand = card.Brand });
        }

        [HttpGet("cards/{cardId}")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Get payment card", Description = "Retrieves a payment card by ID")]
        [SwaggerResponse(200, "Card details")]
        [SwaggerResponse(404, "Card not found")]
        public async Task<IActionResult> GetCard(
            [FromRoute] string cardId,
            [FromHeader(Name = "x-user-id")] string userId)
        {
            var card = await _paymentService.GetCardAsync(cardId, userId);
            if (card == null)
            {
                return NotFound();
            }
            return Ok(new { cardId = card.Id, last4 = card.Last4, brand = card.Brand, isDefault = card.IsDefault });
        }

        [HttpPost("bill")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Bill user", Description = "Charges the user's card. Supports both saved card (cardId) or full card details. WARNING: Accepting raw card details (CardNumber, Cvc) directly violates PCI DSS compliance. For production, use Stripe.js/Elements on the frontend to tokenize cards client-side, then pass only the token to this endpoint. This endpoint is for development/demo purposes only.")]
        [SwaggerRequestExample(typeof(BillingRequestDto), typeof(BillingRequestExample))]
        [SwaggerResponse(200, "Billing successful")]
        [SwaggerResponse(400, "Validation error or payment failed")]
        public async Task<IActionResult> Bill(
            [FromBody] BillingRequestDto request,
            [FromHeader(Name = "x-user-id")] string userId,
            [FromHeader(Name = "x-api-key")] string? apiKey,
            [FromHeader(Name = "x-session-id")] string? sessionId)
        {
            // Validate: either cardId OR full card details must be provided
            bool hasCardId = !string.IsNullOrEmpty(request.CardId);
            bool hasFullCard = !string.IsNullOrEmpty(request.CardNumber) && 
                              request.ExpMonth.HasValue && 
                              request.ExpYear.HasValue && 
                              !string.IsNullOrEmpty(request.Cvc);

            if (!hasCardId && !hasFullCard)
            {
                return BadRequest(new { error = "Either cardId or full card details (cardNumber, expMonth, expYear, cvc) must be provided" });
            }
            
            // Block raw card details in production for PCI DSS compliance
            if (hasFullCard && _configuration.GetValue<bool>("Services:Payment:DisableRawCardDetails", false))
            {
                return BadRequest(new { error = "Raw card details are disabled for security. Please use Stripe.js/Elements on the frontend to tokenize cards, or use a saved cardId." });
            }

            var context = new WorkflowContext 
            { 
                UserId = userId, 
                SessionId = sessionId,
                PaymentId = request.CardId
            };

            var billingRequest = new BillingRequest
            {
                UserId = userId,
                CardId = request.CardId,
                CardNumber = request.CardNumber,
                ExpMonth = request.ExpMonth,
                ExpYear = request.ExpYear,
                Cvc = request.Cvc,
                TotalAmount = request.TotalAmount,
                Currency = request.Currency,
                ApiKey = apiKey
            };

            var result = await _orchestrator.ExecuteAsync("billing", context, billingRequest);
            
            // Use System.Text.Json to safely check result
            if (result.Data != null)
            {
                var jsonElement = System.Text.Json.JsonSerializer.SerializeToElement(result.Data);
                if (jsonElement.TryGetProperty("success", out var successProp) && 
                    !successProp.GetBoolean())
                {
                    return BadRequest(result.Data);
                }
            }
            
            return Ok(result.Data);
        }
    }
}

