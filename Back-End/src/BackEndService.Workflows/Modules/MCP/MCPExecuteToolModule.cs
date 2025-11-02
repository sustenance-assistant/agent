using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Gateway;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;
using BackEndService.Core.Models.MCP;

namespace BackEndService.Workflows.Modules.MCP
{
    public class MCPExecuteToolModule : IWorkflowModule
    {
        private readonly IMCPHandler _handler;
        private readonly IWorkflowOrchestrator _orchestrator;
        private readonly IContextStore _contextStore;
        private readonly IAuthService _authService;

        public MCPExecuteToolModule(IMCPHandler handler, IWorkflowOrchestrator orchestrator, IContextStore contextStore, IAuthService authService)
        {
            _handler = handler;
            _orchestrator = orchestrator;
            _contextStore = contextStore;
            _authService = authService;
        }

        public async Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            // Retrieve user context from JSON store for additional context
            var userContext = await _contextStore.GetAsync(context.SessionId ?? context.UserId);
            if (userContext != null)
            {
                // Merge context: payment info, location, etc.
                if (string.IsNullOrEmpty(context.PaymentId) && !string.IsNullOrEmpty(userContext.PaymentId))
                {
                    context.PaymentId = userContext.PaymentId;
                }
                if (string.IsNullOrEmpty(context.Location) && !string.IsNullOrEmpty(userContext.Location))
                {
                    context.Location = userContext.Location;
                }
            }

            // Validate and inject API key if present
            string? apiKey = null;
            // API key should be passed via headers, but we can store it in context

            if (input is McpToolCall call && !string.IsNullOrWhiteSpace(call.Workflow))
            {
                // Inject API key into arguments if available
                var enrichedArgs = call.Arguments;
                if (apiKey != null && enrichedArgs.HasValue)
                {
                    var argsJson = enrichedArgs.Value.Clone();
                    var argsObj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argsJson.GetRawText()) ?? new Dictionary<string, JsonElement>();
                    argsObj["apiKey"] = JsonDocument.Parse($"\"{apiKey}\"").RootElement;
                    enrichedArgs = JsonDocument.Parse(JsonSerializer.Serialize(argsObj)).RootElement;
                }

                var inner = await _orchestrator.ExecuteAsync(call.Workflow, context, enrichedArgs ?? default);
                return inner.Data ?? new { status = "ok" };
            }

            var fallback = await _handler.CallToolAsync(JsonDocument.Parse("{}"));
            return fallback.RootElement.Clone();
        }
    }
}


