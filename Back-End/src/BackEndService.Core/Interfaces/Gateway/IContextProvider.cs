using System.Threading.Tasks;
using BackEndService.Core.Models.Context;

namespace BackEndService.Core.Interfaces.Gateway
{
    public interface IContextProvider
    {
        Task<WorkflowContext> GetContextAsync(object request);
    }
}
