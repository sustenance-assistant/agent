using System;
using System.Collections.Generic;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Workflows;

namespace BackEndService.Workflows.Orchestration
{
    public class WorkflowRegistry : IWorkflowRepository
    {
        private readonly Dictionary<string, IReadOnlyList<WorkflowStep>> _workflows = new();

        public void Register(string name, IReadOnlyList<WorkflowStep> steps)
        {
            _workflows[name] = steps;
        }

        public IReadOnlyList<WorkflowStep> GetWorkflow(string name)
        {
            if (_workflows.TryGetValue(name, out var steps))
            {
                return steps;
            }

            throw new InvalidOperationException($"Workflow '{name}' is not registered.");
        }
    }
}


