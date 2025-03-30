using TranslationWeb.Core.Constants;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.Translation;

namespace TranslationWeb.Infrastructure.Services
{
    public class TranslationApiService : ITranslationApiService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<TranslationApiService> _logger;

        public TranslationApiService(HttpService httpService, ILogger<TranslationApiService> logger)
        {
            _httpService = httpService;
            _logger = logger;
        }

        public async Task<IEnumerable<Language>> GetLanguagesAsync()
        {
            try
            {
                var languages = await _httpService.GetAsync<IEnumerable<Language>>(ApiEndpoints.Translation.Languages);
                return languages ?? new List<Language>();
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
                var response = await _httpService.PostAsync<TranslationRequest,TranslationResponse>(
                    ApiEndpoints.Translation.Translate,
                    request);
                return response ?? new TranslationResponse
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