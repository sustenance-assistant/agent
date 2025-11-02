using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;

namespace BackEndService.Workflows.Modules.IntentClassification
{
    public class IntentClassificationWorkflowModule : IWorkflowModule
    {
        private readonly IIntentClassifier _classifier;
        private readonly IContextStore _contextStore;

        public IntentClassificationWorkflowModule(IIntentClassifier classifier, IContextStore contextStore)
        {
            _classifier = classifier;
            _contextStore = contextStore;
        }

        public async Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            var text = input?.ToString() ?? string.Empty;
            
            // Retrieve user context for history
            var userContext = await _contextStore.GetAsync(context.SessionId ?? context.UserId);
            var history = userContext != null ? $"User: {context.UserId}, Location: {userContext.Location}" : null;
            
            var classification = await _classifier.ClassifyAsync(text, history);
            
            return new 
            { 
                intent = classification.Intent, 
                confidence = classification.Confidence,
                suggestedWorkflow = classification.Intent == "question" ? "rag" : "order"
            };
        }
    }
}

