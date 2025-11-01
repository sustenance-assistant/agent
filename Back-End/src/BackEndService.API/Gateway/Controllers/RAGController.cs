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
    [Route("api/rag")]
    public class RAGController : ControllerBase
    {
        private readonly IWorkflowOrchestrator _orchestrator;

        public RAGController(IWorkflowOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpPost("search")]
        [EnableRateLimiting("fixed")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "RAG search", Description = "Performs retrieval-augmented generation search")]
        [SwaggerResponse(200, "Search results")]
        public async Task<IActionResult> Search(
            [FromBody] TextStreamRequestDto request,
            [FromHeader(Name = "x-user-id")] string? userId,
            [FromHeader(Name = "x-session-id")] string? sessionId)
        {
            var context = new WorkflowContext 
            { 
                UserId = userId ?? "demo-user",
                SessionId = sessionId ?? "demo-session"
            };

            var result = await _orchestrator.ExecuteAsync("rag", context, request.Text);
            
            return Ok(result.Data);
        }

        [HttpPost("search/dummy")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Test RAG with dummy data", Description = "Returns dummy search results for testing")]
        [SwaggerResponse(200, "Dummy search results")]
        public async Task<IActionResult> SearchDummy(
            [FromHeader(Name = "x-user-id")] string? userId,
            [FromHeader(Name = "x-session-id")] string? sessionId)
        {
            var context = new WorkflowContext 
            { 
                UserId = userId ?? "demo-user",
                SessionId = sessionId ?? "demo-session"
            };

            var result = await _orchestrator.ExecuteAsync("rag", context, "What types of pizza do you offer?");
            
            return Ok(result.Data);
        }
    }
}

