using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Auth;
using BackEndService.Core.Models.Context;

namespace BackEndService.Workflows.Modules.Auth
{
    public class CreateApiKeyWorkflowModule : IWorkflowModule
    {
        private readonly IAuthService _authService;

        public CreateApiKeyWorkflowModule(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            var apiKey = await _authService.CreateApiKeyAsync(context.UserId);
            return apiKey;
        }
    }
}

