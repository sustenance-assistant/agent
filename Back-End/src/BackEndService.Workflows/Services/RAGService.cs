using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;

namespace BackEndService.Workflows.Services
{
    public class RAGService : IRAGService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _embeddingModel;
        private readonly string _baseUrl;

        public RAGService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = _configuration["Services:RAG:ApiKey"] ?? "";
            _model = _configuration["Services:RAG:Model"] ?? "gpt-4";
            _embeddingModel = _configuration["Services:RAG:EmbeddingModel"] ?? "text-embedding-3-small";
            _baseUrl = _configuration["Services:RAG:BaseUrl"] ?? "https://api.openai.com/v1";
        }

        public async Task<string[]> SearchAsync(string query, string? userId = null)
        {
            // Return dummy search results if API key not configured
            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("your-") || _apiKey.Contains("sk-") == false)
            {
                return new[]
                {
                    "We offer pepperoni, margherita, and veggie pizzas.",
                    "Our menu includes large, medium, and small sizes.",
                    "Delivery is available within 5 miles of our location."
                };
            }

            try
            {
                // For now, use ChatGPT completion API to generate RAG-like responses
                // In production, this would:
                // 1. Generate embeddings for query
                // 2. Search vector database
                // 3. Retrieve top-k documents
                // 4. Generate response with context

                var chatUrl = $"{_baseUrl}/chat/completions";
                var request = new HttpRequestMessage(HttpMethod.Post, chatUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var requestBody = new
                {
                    model = _model,
                    messages = new[]
                    {
                        new { role = "system", content = "You are a helpful food ordering assistant. Provide concise, relevant answers about menu items, pricing, and ordering." },
                        new { role = "user", content = query }
                    },
                    max_tokens = 150,
                    temperature = 0.7
                };

                request.Content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(jsonResponse);
                
                if (jsonDoc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    if (firstChoice.TryGetProperty("message", out var message) && 
                        message.TryGetProperty("content", out var content))
                    {
                        var resultText = content.GetString() ?? "";
                        // Split into sentences for multiple results
                        return resultText.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .Take(3)
                            .ToArray();
                    }
                }

                return new[] { "No results found" };
            }
            catch (HttpRequestException ex)
            {
                // Return dummy results on error
                return new[] { $"Dummy result (API error: {ex.Message})" };
            }
        }
    }
}
