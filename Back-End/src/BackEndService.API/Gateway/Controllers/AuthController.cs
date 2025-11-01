using System.Threading.Tasks;
using BackEndService.API.DTO.Auth;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace BackEndService.API.Gateway.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWorkflowOrchestrator _orchestrator;

        public AuthController(IAuthService authService, IWorkflowOrchestrator orchestrator)
        {
            _authService = authService;
            _orchestrator = orchestrator;
        }

        [HttpPost("register")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Register new user", Description = "Creates a new user account")]
        [SwaggerResponse(200, "User created")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var user = await _authService.RegisterAsync(request.Email, request.Password, request.Name);
            return Ok(new { userId = user.Id, email = user.Email });
        }

        [HttpPost("login")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Login user", Description = "Authenticates user and returns session")]
        [SwaggerResponse(200, "Login successful")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var user = await _authService.LoginAsync(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new { error = "Invalid credentials" });
            }

            return Ok(new { userId = user.Id, email = user.Email });
        }

        [HttpPost("api-key")]
        [EnableRateLimiting("fixed")]
        [SwaggerOperation(Summary = "Create API key", Description = "Generates a new API key for the authenticated user")]
        [SwaggerResponse(200, "API key created")]
        public async Task<IActionResult> CreateApiKey([FromHeader(Name = "x-user-id")] string userId)
        {
            var context = new WorkflowContext { UserId = userId };
            var result = await _orchestrator.ExecuteAsync("create-api-key", context, new { });
            return Ok(result.Data);
        }
    }
}

