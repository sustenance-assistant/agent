using System.Threading.Tasks;
using BackEndService.Core.Models.Context;
using BackEndService.Core.Models.Shared;

namespace BackEndService.Core.Interfaces.Services
{
    public interface IIntentClassifier
    {
        Task<IntentClassification> ClassifyAsync(string text, string? userHistory);
    }
}
