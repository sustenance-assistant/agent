using Microsoft.AspNetCore.Mvc;
using BackEndService.Core.Interfaces.Services;

namespace BackEndService.API.Gateway.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly ILogRepository _logs;

        public HealthController(ILogRepository logs)
        {
            _logs = logs;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logs.Info("health_check");
            return Ok(new { status = "ok" });
        }
    }
}


