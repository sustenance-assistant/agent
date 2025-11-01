using BackEndService.API.DTO;
using Swashbuckle.AspNetCore.Filters;

namespace BackEndService.API.Swagger
{
    public class TextStreamRequestExample : IExamplesProvider<TextStreamRequestDto>
    {
        public TextStreamRequestDto GetExamples() => new TextStreamRequestDto
        {
            Text = "I'd like a large pepperoni pizza and a cola."
        };
    }

    public class McpToolCallRequestExample : IExamplesProvider<McpToolCallRequestDto>
    {
        public McpToolCallRequestDto GetExamples() => new McpToolCallRequestDto
        {
            Tool = "order",
            Workflow = "order",
            Arguments = System.Text.Json.JsonDocument.Parse("{\"items\":[{\"sku\":\"pizza.pepperoni.lg\",\"qty\":1}],\"paymentId\":\"pay_123\"}").RootElement
        };
    }
}


