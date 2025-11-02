using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Models.Auth;

namespace BackEndService.Workflows.Services
{
    public class AuthService : IAuthService
    {
        private static readonly ConcurrentDictionary<string, User> Users = new();
        private static readonly ConcurrentDictionary<string, ApiKey> ApiKeys = new();

        public Task<User> RegisterAsync(string email, string password, string? name)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                Name = name
            };
            Users[user.Id] = user;
            return Task.FromResult(user);
        }

        public Task<User?> LoginAsync(string email, string password)
        {
            var user = Users.Values.FirstOrDefault(u => u.Email == email);
            return Task.FromResult(user);
        }

        public Task<ApiKey> CreateApiKeyAsync(string userId)
        {
            var keyBytes = new byte[32];
            RandomNumberGenerator.Fill(keyBytes);
            var apiKey = Convert.ToBase64String(keyBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');

            var key = new ApiKey
            {
                Key = $"sk_{apiKey}",
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            };

            ApiKeys[key.Key] = key;
            return Task.FromResult(key);
        }

        public Task<ApiKey?> ValidateApiKeyAsync(string apiKey)
        {
            if (ApiKeys.TryGetValue(apiKey, out var key) && key.IsActive && (key.ExpiresAt == null || key.ExpiresAt > DateTime.UtcNow))
            {
                return Task.FromResult<ApiKey?>(key);
            }
            return Task.FromResult<ApiKey?>(null);
        }
    }
}

