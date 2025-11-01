using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace BackEndService.API.Gateway.Controllers
{
    [ApiController]
    [Route("api/context")]
    public class ContextController : ControllerBase
    {
        private readonly IContextStore _contextStore;

        public ContextController(IContextStore contextStore)
        {
            _contextStore = contextStore;
        }

        [HttpGet("{sessionId}")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Get workflow context", Description = "Retrieves stored workflow context by sessionId.")]
        [SwaggerResponse(200, "Workflow context for sessionId")]
        public async Task<ActionResult<WorkflowContext?>> Get(string sessionId)
        {
            var ctx = await _contextStore.GetAsync(sessionId);
            return Ok(ctx);
        }
    }
}


