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

        /// <summary>
        /// Registers a new user. NOTE: This is a placeholder implementation for development/demo purposes.
        /// Password is not validated or stored. For production, implement proper password hashing (e.g., BCrypt, Argon2)
        /// and persist users to a secure database.
        /// </summary>
        public Task<User> RegisterAsync(string email, string password, string? name)
        {
            // Validate email is not empty (basic validation for placeholder)
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required", nameof(email));
            }
            
            // Check for duplicate email
            if (Users.Values.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"User with email '{email}' already exists");
            }
            
            // Note: Password parameter is intentionally ignored - this is a demo placeholder
            // In production, hash password with BCrypt/Argon2 and store securely
            
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                Name = name
            };
            Users[user.Id] = user;
            return Task.FromResult(user);
        }

        /// <summary>
        /// Authenticates a user by email. NOTE: This is a placeholder implementation for development/demo purposes.
        /// Password is not verified. For production, implement password hash verification against stored credentials.
        /// This method currently allows authentication bypass and should NOT be used in production.
        /// </summary>
        public Task<User?> LoginAsync(string email, string password)
        {
            // Basic validation for placeholder
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return Task.FromResult<User?>(null);
            }
            
            var user = Users.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            // Note: Password verification intentionally skipped - this is a demo placeholder
            // In production, verify password hash against stored credentials
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

