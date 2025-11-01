using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;

namespace BackEndService.Workflows.Modules.RAG
{
    public class RAGWorkflowModule : IWorkflowModule
    {
        private readonly IRAGService _rag;
        private readonly IRAGRepository? _repository;

        public RAGWorkflowModule(IRAGService rag, IRAGRepository? repository = null)
        {
            _rag = rag;
            _repository = repository;
        }

        public async Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            var text = input?.ToString() ?? string.Empty;
            var results = await _rag.SearchAsync(text, context.UserId);
            
            // Save search results to JSON repository
            if (_repository != null && !string.IsNullOrEmpty(context.UserId))
            {
                await _repository.SaveSearchAsync(context.UserId, text, results);
            }
            
            return new { 
                query = text,
                results = results,
                count = results.Length 
            };
        }
    }
}


