using System.Collections.Concurrent;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Models.Context;

namespace BackEndService.Gateway.Services
{
    public class ContextStore : IContextStore
    {
        private static readonly ConcurrentDictionary<string, WorkflowContext> Store = new();

        public Task SaveAsync(string sessionId, WorkflowContext context)
        {
            Store[sessionId] = context;
            return Task.CompletedTask;
        }

        public Task<WorkflowContext?> GetAsync(string sessionId)
        {
            Store.TryGetValue(sessionId, out var value);
            return Task.FromResult(value);
        }
    }
}


