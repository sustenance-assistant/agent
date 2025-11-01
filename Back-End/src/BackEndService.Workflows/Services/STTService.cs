using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;

namespace BackEndService.Workflows.Services
{
    public class STTService : ISTTService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _baseUrl;

        public STTService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = _configuration["Services:STT:ApiKey"] ?? "";
            _model = _configuration["Services:STT:Model"] ?? "whisper-1";
            _baseUrl = _configuration["Services:STT:BaseUrl"] ?? "https://api.openai.com/v1/audio/transcriptions";
        }

        public async Task<string> TranscribeAsync(Stream audioStream)
        {
            // Return dummy data if API key not configured
            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("your-") || _apiKey.Contains("sk-") == false)
            {
                return "I'd like to order a large pepperoni pizza with extra cheese and a side of garlic bread.";
            }

            try
            {
                // Reset stream position
                audioStream.Position = 0;
                
                var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(audioStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
                content.Add(streamContent, "file", "audio.mp3");
                content.Add(new StringContent(_model), "model");

                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(jsonResponse);
                
                if (jsonDoc.RootElement.TryGetProperty("text", out var textElement))
                {
                    return textElement.GetString() ?? "Transcription failed";
                }

                return "Transcription failed: no text in response";
            }
            catch (HttpRequestException ex)
            {
                // Log error and return dummy data
                return $"Dummy transcription (API error: {ex.Message})";
            }
        }
    }
}
