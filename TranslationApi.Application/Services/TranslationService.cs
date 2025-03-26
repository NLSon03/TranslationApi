using Microsoft.Extensions.Logging;
using TranslationApi.Application.Contracts;
using TranslationApi.Application.Interfaces;

namespace TranslationApi.Application.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly ILogger<TranslationService> _logger;

        public TranslationService(ILogger<TranslationService> logger)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<Language>> GetSupportedLanguagesAsync()
        {
            var languages = new List<Language>
            {
                new Language { Code = "en", Name = "English" },
                new Language { Code = "vi", Name = "Vietnamese" },
                new Language { Code = "zh", Name = "Chinese" },
                new Language { Code = "ja", Name = "Japanese" },
                new Language { Code = "ko", Name = "Korean" },
                new Language { Code = "fr", Name = "French" },
                new Language { Code = "de", Name = "German" },
                new Language { Code = "es", Name = "Spanish" },
                new Language { Code = "ru", Name = "Russian" }
            };

            return await Task.FromResult(languages);
        }

        public async Task<TranslationResponse> TranslateTextAsync(TranslationRequest request)
        {
            _logger.LogInformation($"Translating text from {request.SourceLanguage} to {request.TargetLanguage}");

            return await Task.FromResult(new TranslationResponse
            {
                TranslatedText = "This is a placeholder translation. Will be implemented with Gemini API.",
                SourceLanguage = request.SourceLanguage,
                TargetLanguage = request.TargetLanguage,
                Success = true
            });
        }


    }
}
