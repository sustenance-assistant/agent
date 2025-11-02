using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace BackEndService.Infrastructure.Storage
{
    public class JsonSTTRepository : ISTTRepository
    {
        private readonly string _storagePath;

        public JsonSTTRepository(IConfiguration configuration)
        {
            var basePath = configuration["Data:StoragePath"] ?? "data";
            _storagePath = Path.Combine(basePath, "stt");
            Directory.CreateDirectory(_storagePath);
        }

        public async Task SaveTranscriptionAsync(string sessionId, string transcription)
        {
            var filePath = Path.Combine(_storagePath, $"{sessionId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            var data = new { sessionId, transcription, timestamp = DateTime.UtcNow };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}

