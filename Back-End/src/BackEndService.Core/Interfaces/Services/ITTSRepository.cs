using System.IO;
using System.Threading.Tasks;

namespace BackEndService.Core.Interfaces.Services
{
    public interface ITTSRepository
    {
        Task SaveAudioAsync(string sessionId, Stream audioStream, string text);
    }
}

