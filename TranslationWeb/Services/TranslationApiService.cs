using System.Text.Json;
using TranslationWeb.Models.Api;

namespace TranslationWeb.Services
{
    public class TranslationApiService : ITranslationApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TranslationApiService> _logger;
        private readonly string _apiBaseUrl;

        public TranslationApiService(HttpClient httpClient, IConfiguration configuration, ILogger<TranslationApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiBaseUrl = "http://localhost:5292";
        }

        public async Task<IEnumerable<Language>> GetLanguagesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Translation/languages");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var language = JsonSerializer.Deserialize<IEnumerable<Language>>(
                    content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                return language ?? new List<Language>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching languages from API");
                return new List<Language>();
            }
        }

        public async Task<TranslationResponse> TranslateTextAsync(TranslationRequest request)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json"
                    );
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Translation", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var translationResponse = JsonSerializer.Deserialize<TranslationResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return translationResponse ?? new TranslationResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to deserialize response"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error translating text via API");
                return new TranslationResponse
                {
                    Success = false,
                    ErrorMessage = $"Translation failed: {ex.Message}",
                    SourceLanguage = request.SourceLanguage,
                    TargetLanguage = request.TargetLanguage
                };
            }
        }
    }
}
