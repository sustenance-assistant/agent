using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using BackEndService.Core.Models.Context;

namespace BackEndService.Infrastructure.Storage
{
    public class JsonContextStore : IContextStore
    {
        private readonly string _dataPath;
        private readonly ConcurrentDictionary<string, WorkflowContext> _cache = new();

        public JsonContextStore()
        {
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "context");
            Directory.CreateDirectory(_dataPath);
        }

        public async Task SaveAsync(string sessionId, WorkflowContext context)
        {
            _cache[sessionId] = context;
            var filePath = Path.Combine(_dataPath, $"{sessionId}.json");
            var json = JsonSerializer.Serialize(context, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<WorkflowContext?> GetAsync(string sessionId)
        {
            if (_cache.TryGetValue(sessionId, out var cached))
            {
                return cached;
            }

            var filePath = Path.Combine(_dataPath, $"{sessionId}.json");
            if (!File.Exists(filePath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var context = JsonSerializer.Deserialize<WorkflowContext>(json);
            if (context != null)
            {
                _cache[sessionId] = context;
            }

            return context;
        }
    }
}

