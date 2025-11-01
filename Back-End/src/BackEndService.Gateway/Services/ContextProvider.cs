using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Gateway;
using BackEndService.Core.Models.Context;

namespace BackEndService.Gateway.Services
{
    public class ContextProvider : IContextProvider
    {
        public Task<WorkflowContext> GetContextAsync(object request)
        {
            // Placeholder: map request to context. In real impl, inspect request body/headers.
            return Task.FromResult(new WorkflowContext { UserId = string.Empty });
        }
    }
}


