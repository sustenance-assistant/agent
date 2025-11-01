using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Gateway;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;

namespace BackEndService.Workflows.Modules.MCP
{
    public class MCPListToolsModule : IWorkflowModule
    {
        private readonly IMCPHandler _handler;

        public MCPListToolsModule(IMCPHandler handler)
        {
            _handler = handler;
        }

        public async Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            var result = await _handler.ListToolsAsync(System.Text.Json.JsonDocument.Parse("{}"));
            return result.RootElement.Clone();
        }
    }
}


