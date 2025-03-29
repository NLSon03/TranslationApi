using TranslationWeb.Models.Translation;

namespace TranslationWeb.Infrastructure.Services.Api
{
    public interface ITranslationApiService
    {
        Task<IEnumerable<Language>> GetLanguagesAsync();
        Task<TranslationResponse> TranslateTextAsync(TranslationRequest request);
    }
} 