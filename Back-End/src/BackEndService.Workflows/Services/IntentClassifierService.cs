using System.Linq;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Models.Shared;

namespace BackEndService.Workflows.Services
{
    public class IntentClassifierService : IIntentClassifier
    {
        public Task<IntentClassification> ClassifyAsync(string text, string? userHistory)
        {
            var lower = text.ToLowerInvariant();
            
            // Simple keyword-based classification
            var orderKeywords = new[] { "order", "buy", "purchase", "add to cart", "checkout", "pay" };
            var questionKeywords = new[] { "what", "how", "where", "when", "why", "?", "tell me", "show me", "find" };

            var isOrder = orderKeywords.Any(k => lower.Contains(k));
            var isQuestion = questionKeywords.Any(k => lower.Contains(k));

            string intent;
            double confidence;

            if (isOrder && !isQuestion)
            {
                intent = "order";
                confidence = 0.85;
            }
            else if (isQuestion || (!isOrder && !isQuestion))
            {
                intent = "question";
                confidence = isQuestion ? 0.80 : 0.60;
            }
            else
            {
                // Ambiguous - default to question
                intent = "question";
                confidence = 0.65;
            }

            return Task.FromResult(new IntentClassification { Intent = intent, Confidence = confidence });
        }
    }
}

