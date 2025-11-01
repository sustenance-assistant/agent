using System.Threading.Tasks;
using BackEndService.API.DTO;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace BackEndService.API.Gateway.Controllers
{
    [ApiController]
    [Route("api/tts")]
    public class TTSController : ControllerBase
    {
        private readonly IWorkflowOrchestrator _orchestrator;

        public TTSController(IWorkflowOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpPost("synthesize")]
        [EnableRateLimiting("fixed")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Synthesize text to speech", Description = "Converts text to audio using OpenAI TTS")]
        [SwaggerResponse(200, "Audio generated")]
        public async Task<IActionResult> Synthesize(
            [FromBody] TextStreamRequestDto request,
            [FromHeader(Name = "x-user-id")] string? userId,
            [FromHeader(Name = "x-session-id")] string? sessionId)
        {
            var context = new WorkflowContext 
            { 
                UserId = userId ?? "demo-user",
                SessionId = sessionId ?? "demo-session"
            };

            var result = await _orchestrator.ExecuteAsync("tts", context, request.Text);
            
            return Ok(result.Data);
        }

        [HttpPost("synthesize/dummy")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Test TTS with dummy data", Description = "Returns dummy audio data for testing")]
        [SwaggerResponse(200, "Dummy audio data")]
        public async Task<IActionResult> SynthesizeDummy(
            [FromHeader(Name = "x-user-id")] string? userId,
            [FromHeader(Name = "x-session-id")] string? sessionId)
        {
            var context = new WorkflowContext 
            { 
                UserId = userId ?? "demo-user",
                SessionId = sessionId ?? "demo-session"
            };

            var result = await _orchestrator.ExecuteAsync("tts", context, "Hello, this is a test message.");
            
            return Ok(result.Data);
        }
    }
}

