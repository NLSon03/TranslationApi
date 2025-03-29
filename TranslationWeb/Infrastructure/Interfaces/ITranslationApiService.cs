using TranslationWeb.Models.Translation;

namespace TranslationWeb.Infrastructure.Interfaces
{
    public interface ITranslationApiService
    {
        Task<IEnumerable<Language>> GetLanguagesAsync();
        Task<TranslationResponse> TranslateTextAsync(TranslationRequest request);
    }
} 