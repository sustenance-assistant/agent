using System.Threading.Tasks;

namespace BackEndService.Core.Interfaces.Services
{
    public interface ISTTRepository
    {
        Task SaveTranscriptionAsync(string sessionId, string transcription);
    }
}

