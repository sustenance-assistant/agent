using System.Threading.Tasks;
using BackEndService.API.DTO;
using BackEndService.Core.Interfaces.Gateway;
using BackEndService.Core.Interfaces.Workflows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace BackEndService.API.Gateway.Controllers
{
    [ApiController]
    [Route("api/mcp")]
    public class MCPGatewayController : ControllerBase
    {
        private readonly IMCPHandler _mcpHandler;
        private readonly IContextProvider _contextProvider;
        private readonly IWorkflowOrchestrator _orchestrator;

        public MCPGatewayController(IMCPHandler mcpHandler, IContextProvider contextProvider, IWorkflowOrchestrator orchestrator)
        {
            _mcpHandler = mcpHandler;
            _contextProvider = contextProvider;
            _orchestrator = orchestrator;
        }

        [HttpPost("tools/list")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "List MCP tools", Description = "Returns the list of available MCP tools.")]
        [SwaggerResponse(200, "List of tools")]
        public async Task<IActionResult> ListTools()
        {
            var context = await _contextProvider.GetContextAsync(new { });
            var result = await _orchestrator.ExecuteAsync("mcp-list-tools", context, new { });
            return Ok(result.Data);
        }

        [HttpPost("tools/call")]
        [EnableRateLimiting("fixed")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Call an MCP tool", Description = "Invokes a tool by name with arguments. API key is injected from x-api-key header.")]
        [SwaggerResponse(200, "Tool invocation result")]
        [SwaggerRequestExample(typeof(McpToolCallRequestDto), typeof(BackEndService.API.Swagger.McpToolCallRequestExample))]
        public async Task<IActionResult> CallTool(
            [FromBody] McpToolCallRequestDto request,
            [FromHeader(Name = "x-api-key")] string? apiKey,
            [FromHeader(Name = "x-user-id")] string? userId,
            [FromHeader(Name = "x-session-id")] string? sessionId)
        {
            var context = await _contextProvider.GetContextAsync(request);
            context.UserId = userId ?? context.UserId;
            context.SessionId = sessionId ?? context.SessionId;
            
            var model = new BackEndService.Core.Models.MCP.McpToolCall
            {
                Tool = request.Tool,
                Workflow = request.Workflow,
                Arguments = request.Arguments
            };
            
            // Inject API key into context for MCP module to use
            if (!string.IsNullOrEmpty(apiKey))
            {
                // Store in HttpContext for middleware/context retrieval
                HttpContext.Items["ApiKey"] = apiKey;
            }
            
            var result = await _orchestrator.ExecuteAsync("mcp-execute-tool", context, model);
            return Ok(result.Data);
        }
    }
}


