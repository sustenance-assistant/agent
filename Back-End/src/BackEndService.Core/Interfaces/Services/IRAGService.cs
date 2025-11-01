using System.Threading.Tasks;

namespace BackEndService.Core.Interfaces.Services
{
    public interface IRAGService
    {
        Task<string[]> SearchAsync(string query, string? userId = null);
    }
}


