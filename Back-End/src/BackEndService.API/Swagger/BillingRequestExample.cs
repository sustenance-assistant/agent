using BackEndService.API.DTO.Payment;
using Swashbuckle.AspNetCore.Filters;

namespace BackEndService.API.Swagger
{
    public class BillingRequestExample : IExamplesProvider<BillingRequestDto>
    {
        public BillingRequestDto GetExamples() => new BillingRequestDto
        {
            CardNumber = "4242424242424242",
            ExpMonth = 12,
            ExpYear = 2025,
            Cvc = "123",
            TotalAmount = 29.99m,
            Currency = "USD"
        };
    }

    public class BillingRequestWithCardIdExample : IExamplesProvider<BillingRequestDto>
    {
        public BillingRequestDto GetExamples() => new BillingRequestDto
        {
            CardId = "card_1234567890",
            TotalAmount = 29.99m,
            Currency = "USD"
        };
    }
}

