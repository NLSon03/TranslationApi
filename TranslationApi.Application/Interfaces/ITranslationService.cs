using TranslationApi.Application.Contracts;

namespace TranslationApi.Application.Interfaces
{
    public interface ITranslationService
    {
        Task<TranslationResponse> TranslateTextAsync(TranslationRequest request);
        Task<IEnumerable<Language>> GetSupportedLanguagesAsync();
    }
}
