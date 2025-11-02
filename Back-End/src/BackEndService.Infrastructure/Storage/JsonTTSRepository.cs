using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace BackEndService.Infrastructure.Storage
{
    public class JsonTTSRepository : ITTSRepository
    {
        private readonly string _storagePath;

        public JsonTTSRepository(IConfiguration configuration)
        {
            var basePath = configuration["Data:StoragePath"] ?? "data";
            _storagePath = Path.Combine(basePath, "tts");
            Directory.CreateDirectory(_storagePath);
        }

        public async Task SaveAudioAsync(string sessionId, Stream audioStream, string text)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var audioPath = Path.Combine(_storagePath, $"{sessionId}_{timestamp}.mp3");
            var metaPath = Path.Combine(_storagePath, $"{sessionId}_{timestamp}.json");

            // Save audio file - reset stream position before copying
            if (audioStream.CanSeek && audioStream.Position > 0)
            {
                audioStream.Position = 0;
            }
            using var fileStream = File.Create(audioPath);
            await audioStream.CopyToAsync(fileStream);

            // Save metadata
            var metadata = new { sessionId, text, timestamp = DateTime.UtcNow, audioFile = Path.GetFileName(audioPath) };
            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metaPath, json);
        }
    }
}

