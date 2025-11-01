namespace BackEndService.Core.Interfaces.Services
{
    public interface ILogRepository
    {
        void Info(string message, object? context = null);
        void Warning(string message, object? context = null);
        void Error(string message, System.Exception ex, object? context = null);
    }
}
