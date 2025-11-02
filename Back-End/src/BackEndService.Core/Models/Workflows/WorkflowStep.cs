using System;

namespace BackEndService.Core.Models.Workflows
{
    public class WorkflowStep
    {
        public Type ModuleType { get; set; }

        public WorkflowStep(Type moduleType)
        {
            ModuleType = moduleType;
        }
    }
}


