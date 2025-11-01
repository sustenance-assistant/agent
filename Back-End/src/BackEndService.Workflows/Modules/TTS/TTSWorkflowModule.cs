using System.IO;
using System.Text;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;

namespace BackEndService.Workflows.Modules.TTS
{
    public class TTSWorkflowModule : IWorkflowModule
    {
        private readonly ITTSService _tts;
        private readonly ITTSRepository? _repository;

        public TTSWorkflowModule(ITTSService tts, ITTSRepository? repository = null)
        {
            _tts = tts;
            _repository = repository;
        }

        public async Task<object> ExecuteAsync(WorkflowContext context, object input)
        {
            var text = input?.ToString() ?? string.Empty;
            var audio = await _tts.SynthesizeAsync(text);
            
            // Save audio to JSON repository
            if (_repository != null && !string.IsNullOrEmpty(context.SessionId))
            {
                await _repository.SaveAudioAsync(context.SessionId, audio, text);
            }
            
            return new { 
                length = audio.Length, 
                text = text,
                sessionId = context.SessionId 
            };
        }
    }
}


