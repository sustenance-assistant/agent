using System.IO;
using System.Text;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;

namespace BackEndService.Workflows.Modules.Text
{
    public class TextToResponseWorkflowModule : IWorkflowModule
    {
        private readonly IWorkflowOrchestrator _orchestrator;

        public TextToResponseWorkflowModule(IWorkflowOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        public async Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            // Extract text from stream
            string text = string.Empty;
            if (input is Stream s)
            {
                using var ms = new MemoryStream();
                await s.CopyToAsync(ms);
                text = Encoding.UTF8.GetString(ms.ToArray());
            }
            else if (input is string str)
            {
                text = str;
            }
            else
            {
                text = input?.ToString() ?? string.Empty;
            }

            // Classify intent
            var classification = await _orchestrator.ExecuteAsync("intent-classification", context, text);
            dynamic classificationResult = classification.Data ?? new { intent = "question", suggestedWorkflow = "rag" };
            
            string intent = classificationResult.intent?.ToString() ?? "question";
            string suggestedWorkflow = classificationResult.suggestedWorkflow?.ToString() ?? "rag";

            // Route to appropriate workflow
            object? workflowResult = null;
            if (suggestedWorkflow == "rag")
            {
                workflowResult = await _orchestrator.ExecuteAsync("rag", context, text);
            }
            else if (suggestedWorkflow == "order")
            {
                workflowResult = await _orchestrator.ExecuteAsync("order", context, text);
            }

            return new 
            { 
                message = "ok", 
                echo = text,
                intent = intent,
                workflow = suggestedWorkflow,
                result = workflowResult
            };
        }
    }
}
