using System.IO;
using System.Threading.Tasks;

namespace BackEndService.Core.Interfaces.Services
{
    public interface ITTSService
    {
        Task<Stream> SynthesizeAsync(string text);
    }
}
