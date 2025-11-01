using System.Collections.Generic;
using BackEndService.Core.Models.Workflows;

namespace BackEndService.Core.Interfaces.Workflows
{
    public interface IWorkflowRepository
    {
        IReadOnlyList<WorkflowStep> GetWorkflow(string name);
    }
}
