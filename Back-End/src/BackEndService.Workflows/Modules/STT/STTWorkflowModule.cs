using System.IO;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;

namespace BackEndService.Workflows.Modules.STT
{
    public class STTWorkflowModule : IWorkflowModule
    {
        private readonly ISTTService _stt;
        private readonly ISTTRepository? _repository;

        public STTWorkflowModule(ISTTService stt, ISTTRepository? repository = null)
        {
            _stt = stt;
            _repository = repository;
        }

        public async Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            if (input is Stream audio)
            {
                var text = await _stt.TranscribeAsync(audio);
                
                // Save transcription to JSON repository
                if (_repository != null && !string.IsNullOrEmpty(context.SessionId))
                {
                    await _repository.SaveTranscriptionAsync(context.SessionId, text);
                }
                
                return new { transcription = text, sessionId = context.SessionId };
            }
            return new { transcription = string.Empty };
        }
    }
}


