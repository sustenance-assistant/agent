using System.IO;
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
    public class TTSService : ITTSService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _voice;
        private readonly string _baseUrl;

        public TTSService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = _configuration["Services:TTS:ApiKey"] ?? "";
            _model = _configuration["Services:TTS:Model"] ?? "tts-1";
            _voice = _configuration["Services:TTS:Voice"] ?? "alloy";
            _baseUrl = _configuration["Services:TTS:BaseUrl"] ?? "https://api.openai.com/v1/audio/speech";
        }

        public async Task<Stream> SynthesizeAsync(string text)
        {
            // Return dummy audio data if API key not configured
            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("your-") || _apiKey.Contains("sk-") == false)
            {
                var dummyAudio = new byte[] { 0xFF, 0xFB, 0x90, 0x00 }; // MP3 header
                return new MemoryStream(dummyAudio);
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var requestBody = new
                {
                    model = _model,
                    input = text,
                    voice = _voice
                };

                request.Content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var audioStream = await response.Content.ReadAsStreamAsync();
                var memoryStream = new MemoryStream();
                await audioStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                
                return memoryStream;
            }
            catch (HttpRequestException)
            {
                // Return dummy audio on error
                var dummyAudio = new byte[] { 0xFF, 0xFB, 0x90, 0x00 };
                return new MemoryStream(dummyAudio);
            }
        }
    }
}
