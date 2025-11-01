using System.Linq;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BackEndService.API.Gateway.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("x-api-key", out var apiKeyValue))
            {
                // Resolve scoped service from request scope
                var authService = context.RequestServices.GetRequiredService<IAuthService>();
                var apiKey = await authService.ValidateApiKeyAsync(apiKeyValue.ToString());
                if (apiKey != null)
                {
                    context.Items["ApiKey"] = apiKey;
                    context.Items["UserId"] = apiKey.UserId;
                }
            }

            await _next(context);
        }
    }
}

