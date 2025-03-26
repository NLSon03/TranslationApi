using TranslationWeb.Models.Api;

namespace TranslationWeb.Services
{
    public interface ITranslationApiService
    {
        Task<IEnumerable<Language>> GetLanguagesAsync();
        Task<TranslationResponse> TranslateTextAsync(TranslationRequest request);
    }
}
