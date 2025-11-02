using System.IO;
using System.Threading.Tasks;

namespace BackEndService.Core.Interfaces.Services
{
    public interface ISTTService
    {
        Task<string> TranscribeAsync(Stream audioStream);
    }
}
