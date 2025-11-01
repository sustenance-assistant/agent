using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BackEndService.API.Gateway.Middleware
{
    public class StreamProcessingMiddleware
    {
        private readonly RequestDelegate _next;

        public StreamProcessingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            return _next(context);
        }
    }
}


