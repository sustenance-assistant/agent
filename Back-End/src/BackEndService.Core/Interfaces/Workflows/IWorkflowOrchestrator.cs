using System.Threading.Tasks;
using BackEndService.Core.Models.Context;
using BackEndService.Core.Models.Workflows;

namespace BackEndService.Core.Interfaces.Workflows
{
    public interface IWorkflowOrchestrator
    {
        Task<WorkflowResponse> ExecuteAsync(string workflowName, WorkflowContext context, object input);
    }
}
