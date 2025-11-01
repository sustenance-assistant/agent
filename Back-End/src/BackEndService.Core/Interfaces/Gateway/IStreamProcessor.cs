using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BackEndService.Core.Models.Context;

namespace BackEndService.Core.Interfaces.Gateway
{
    public interface IStreamProcessor
    {
        Task<WorkflowContext> ExtractContextAsync(IDictionary<string, string> headers);
        Task<Stream> NormalizeAudioAsync(Stream inputAudioStream);
        Task<Stream> NormalizeTextAsync(Stream inputTextStream);
    }
}

