using TranslationApi.Models;

namespace TranslationApi.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly ILogger<TranslationService> _logger;
        private readonly List<CustomTerm> _customTerms = new List<CustomTerm>();

        public TranslationService(ILogger<TranslationService> logger)
        {
            _logger = logger;

            _customTerms.Add(new CustomTerm
            {
                Id = 1,
                SourceTerm = "hello",
                TargetTerm = "xin chào",
                SourceLanguage = "en",
                TargetLanguage = "vi"
            });
        }

        public async Task<CustomTerm> AddCustomTermAsync(CustomTerm term)
        {
            term.Id = _customTerms.Count > 0 ? _customTerms.Max(t => t.Id) + 1 : 1;
            _customTerms.Add(term);
            return await Task.FromResult(term);
        }

        public async Task<bool> DeleteCustomTermAsync(int id)
        {
            var term = _customTerms.FirstOrDefault(x => x.Id == id);
            if (term != null)
            {
                _customTerms.Remove(term);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<IEnumerable<CustomTerm>> GetCustomTermsAsync()
        {
            return await Task.FromResult(_customTerms);
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
