using TranslationApi.Models;

namespace TranslationApi.Services
{
    public interface ITranslationService
    {
        Task<TranslationResponse> TranslateTextAsync(TranslationRequest request);
        Task<IEnumerable<Language>> GetSupportedLanguagesAsync();
        Task<IEnumerable<CustomTerm>> GetCustomTermsAsync();
        Task<CustomTerm> AddCustomTermAsync(CustomTerm term);
        Task<bool> DeleteCustomTermAsync(int id);
    }
}
