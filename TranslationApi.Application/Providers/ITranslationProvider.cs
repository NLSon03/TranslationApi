
using TranslationApi.Application.Contracts;

namespace TranslationApi.Application.Providers
{
    public interface ITranslationProvider : IModelProvider
    {
        Task<TranslationResponse> TranslateAsync(TranslationRequest request);
        Task<IEnumerable<Language>> GetSupportedLanguagesAsync();
    }
}