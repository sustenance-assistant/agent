using System.Threading.Tasks;
using BackEndService.Core.Models.Context;

namespace BackEndService.Core.Interfaces.Services
{
    public interface IContextStore
    {
        Task SaveAsync(string sessionId, WorkflowContext context);
        Task<WorkflowContext?> GetAsync(string sessionId);
    }
}
