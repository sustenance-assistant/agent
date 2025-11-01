using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace BackEndService.Infrastructure.Storage
{
    public class JsonRAGRepository : IRAGRepository
    {
        private readonly string _storagePath;

        public JsonRAGRepository(IConfiguration configuration)
        {
            var basePath = configuration["Data:StoragePath"] ?? "data";
            _storagePath = Path.Combine(basePath, "rag");
            Directory.CreateDirectory(_storagePath);
        }

        public async Task SaveSearchAsync(string userId, string query, string[] results)
        {
            var filePath = Path.Combine(_storagePath, $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            var data = new { userId, query, results, timestamp = DateTime.UtcNow };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}

