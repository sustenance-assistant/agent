using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;

namespace BackEndService.Workflows.Modules.OrderProcessing
{
    public class OrderWorkflowModule : IWorkflowModule
    {
        public Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            var payload = input as JsonElement?;
            return Task.FromResult<object>(new { status = "ordered", payload });
        }
    }
}


