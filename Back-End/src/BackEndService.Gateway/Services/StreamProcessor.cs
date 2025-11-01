using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Gateway;
using BackEndService.Core.Models.Context;

namespace BackEndService.Gateway.Services
{
    public class StreamProcessor : IStreamProcessor
    {
        public Task<WorkflowContext> ExtractContextAsync(IDictionary<string, string> headers)
        {
            var context = new WorkflowContext
            {
                UserId = headers.TryGetValue("x-user-id", out var user) ? user : string.Empty,
                PaymentId = headers.TryGetValue("x-payment-id", out var pid) ? pid : null,
                Location = headers.TryGetValue("x-location", out var loc) ? loc : null,
                SessionId = headers.TryGetValue("x-session-id", out var sid) ? sid : null
            };

            return Task.FromResult(context);
        }

        public Task<Stream> NormalizeAudioAsync(Stream inputAudioStream)
        {
            return Task.FromResult(inputAudioStream);
        }

        public Task<Stream> NormalizeTextAsync(Stream inputTextStream)
        {
            return Task.FromResult(inputTextStream);
        }
    }
}


