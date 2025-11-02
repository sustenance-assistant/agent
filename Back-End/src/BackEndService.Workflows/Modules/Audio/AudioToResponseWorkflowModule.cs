using System.IO;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;

namespace BackEndService.Workflows.Modules.Audio
{
    public class AudioToResponseWorkflowModule : IWorkflowModule
    {
        public Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            if (input is Stream)
            {
                return Task.FromResult<object>(new { message = "audio-ok" });
            }

            return Task.FromResult<object>(new { message = "no-audio" });
        }
    }
}


