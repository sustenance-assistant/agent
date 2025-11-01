using System.Threading.Tasks;
using BackEndService.Core.Models.Auth;

namespace BackEndService.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(string email, string password, string? name);
        Task<User?> LoginAsync(string email, string password);
        Task<ApiKey> CreateApiKeyAsync(string userId);
        Task<ApiKey?> ValidateApiKeyAsync(string apiKey);
    }
}

