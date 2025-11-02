using System.Threading.Tasks;
using BackEndService.Core.Models.Context;

namespace BackEndService.Core.Interfaces.Workflows
{
    public interface IWorkflowModule
    {
        Task<object> ExecuteAsync(WorkflowContext context, object input);
    }
}
