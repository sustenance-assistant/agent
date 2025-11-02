using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;
using BackEndService.Core.Models.Workflows;

namespace BackEndService.Workflows.Orchestration
{
    public class WorkflowOrchestrator : IWorkflowOrchestrator
    {
        private readonly System.IServiceProvider _serviceProvider;
        private readonly IWorkflowRepository _repository;

        public WorkflowOrchestrator(System.IServiceProvider serviceProvider, IWorkflowRepository repository)
        {
            _serviceProvider = serviceProvider;
            _repository = repository;
        }

        public async Task<WorkflowResponse> ExecuteAsync(string workflowName, WorkflowContext context, object input)
        {
            var steps = _repository.GetWorkflow(workflowName);
            object current = input;

            foreach (var step in steps)
            {
                var module = (IWorkflowModule)_serviceProvider.GetRequiredService(step.ModuleType);
                current = await module.ExecuteAsync(context, current);
            }

            return new WorkflowResponse { Data = current };
        }
    }
}


