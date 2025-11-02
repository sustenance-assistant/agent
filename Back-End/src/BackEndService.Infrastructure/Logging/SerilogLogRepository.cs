using BackEndService.Core.Interfaces.Services;
using Serilog;

namespace BackEndService.Infrastructure.Logging
{
    public class SerilogLogRepository : ILogRepository
    {
        public void Info(string message, object? context = null)
        {
            if (context is null) Log.Information(message);
            else Log.Information("{Message} {@Context}", message, context);
        }

        public void Warning(string message, object? context = null)
        {
            if (context is null) Log.Warning(message);
            else Log.Warning("{Message} {@Context}", message, context);
        }

        public void Error(string message, System.Exception ex, object? context = null)
        {
            if (context is null) Log.Error(ex, message);
            else Log.Error(ex, "{Message} {@Context}", message, context);
        }
    }
}


