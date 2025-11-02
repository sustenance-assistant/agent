using System.Threading.Tasks;

namespace BackEndService.Core.Interfaces.Services
{
    public interface IRAGRepository
    {
        Task SaveSearchAsync(string userId, string query, string[] results);
    }
}

