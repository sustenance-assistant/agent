using System;

namespace BackEndService.Core.Models.Shared
{
    public class LogEvent
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public string Level { get; set; } = "Information";
        public string Message { get; set; } = string.Empty;
        public object? Context { get; set; }
        public Exception? Exception { get; set; }
    }
}


