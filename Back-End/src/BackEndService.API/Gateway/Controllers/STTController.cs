using System.IO;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace BackEndService.API.Gateway.Controllers
{
    [ApiController]
    [Route("api/stt")]
    public class STTController : ControllerBase
    {
        private readonly IWorkflowOrchestrator _orchestrator;

        public STTController(IWorkflowOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpPost("transcribe")]
        [EnableRateLimiting("fixed")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Transcribe audio to text", Description = "Accepts audio file and returns transcribed text using OpenAI Whisper")]
        [SwaggerResponse(200, "Transcription successful")]
        public async Task<IActionResult> Transcribe(
            [FromForm] IFormFile audio,
            [FromHeader(Name = "x-user-id")] string? userId,
            [FromHeader(Name = "x-session-id")] string? sessionId)
        {
            var context = new WorkflowContext 
            { 
                UserId = userId ?? "demo-user",
                SessionId = sessionId ?? "demo-session"
            };

            using var stream = audio.OpenReadStream();
            var result = await _orchestrator.ExecuteAsync("stt", context, stream);
            
            return Ok(result.Data);
        }

        [HttpPost("transcribe/dummy")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Test transcription with dummy data", Description = "Returns dummy transcription for testing")]
        [SwaggerResponse(200, "Dummy transcription")]
        public async Task<IActionResult> TranscribeDummy(
            [FromHeader(Name = "x-user-id")] string? userId,
            [FromHeader(Name = "x-session-id")] string? sessionId)
        {
            var context = new WorkflowContext 
            { 
                UserId = userId ?? "demo-user",
                SessionId = sessionId ?? "demo-session"
            };

            // Create dummy audio stream (empty for testing)
            using var dummyStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("dummy audio"));
            var result = await _orchestrator.ExecuteAsync("stt", context, dummyStream);
            
            return Ok(result.Data);
        }
    }
}

