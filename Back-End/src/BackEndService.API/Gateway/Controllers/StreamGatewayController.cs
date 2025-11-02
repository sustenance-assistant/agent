using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BackEndService.API.DTO;
using BackEndService.API.Infrastructure;
using BackEndService.Core.Interfaces.Gateway;
using BackEndService.Core.Interfaces.Workflows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace BackEndService.API.Gateway.Controllers
{
    [ApiController]
    [Route("api/stream")]
    public class StreamGatewayController : ControllerBase
    {
        private readonly IStreamProcessor _streamProcessor;
        private readonly IWorkflowOrchestrator _orchestrator;

        public StreamGatewayController(IStreamProcessor streamProcessor, IWorkflowOrchestrator orchestrator)
        {
            _streamProcessor = streamProcessor;
            _orchestrator = orchestrator;
        }

        [HttpPost("audio")]
        [EnableRateLimiting("fixed")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Submit audio stream", Description = "Accepts an audio file upload and routes through the audio-to-response workflow.")]
        [SwaggerResponse(200, "Audio response")]
        public async Task<IActionResult> ProcessAudioStream([FromForm] AudioStreamRequestDto request)
        {
            var headers = HttpContext.Request.Headers.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value.ToString());
            var context = await _streamProcessor.ExtractContextAsync(headers);
            using Stream input = request.File.OpenReadStream();
            var normalized = await _streamProcessor.NormalizeAudioAsync(input);
            var result = await _orchestrator.ExecuteAsync("audio-to-response", context, normalized);
            return Ok(result.Data);
        }

        [HttpPost("text")]
        [EnableRateLimiting("fixed")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Submit text stream", Description = "Accepts text input and routes through the text-to-response workflow.")]
        [SwaggerResponse(200, "Text response")]
        [SwaggerRequestExample(typeof(TextStreamRequestDto), typeof(BackEndService.API.Swagger.TextStreamRequestExample))]
        public async Task<IActionResult> ProcessTextStream([FromBody] TextStreamRequestDto request)
        {
            var headers = HttpContext.Request.Headers.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value.ToString());
            var context = await _streamProcessor.ExtractContextAsync(headers);
            var sanitized = Sanitizer.SanitizeText(request.Text);
            using var input = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(sanitized));
            var normalized = await _streamProcessor.NormalizeTextAsync(input);
            var result = await _orchestrator.ExecuteAsync("text-to-response", context, normalized);
            return Ok(result.Data);
        }
    }
}


