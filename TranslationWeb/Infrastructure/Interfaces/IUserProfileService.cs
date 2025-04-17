using System.Threading.Tasks;
using TranslationWeb.Models;
using TranslationWeb.Models.Translation;

namespace TranslationWeb.Infrastructure.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileModel?> GetCurrentUserProfileAsync();
        Task<string?> UploadAvatarAsync(Stream fileStream, string fileName);
        Task<List<Language>> GetFrequentlyUsedLanguagesAsync();
        Task UpdateProfileAsync(UserProfileModel profile);
        Task UpdatePreferencesAsync(UserProfileModel profile);
        Task UpdateLastActiveAsync(UserProfileModel profile);
        Task IncrementTranslationCountAsync(UserProfileModel profile);
        Task UpdateFrequentlyUsedLanguagesAsync(UserProfileModel profile);
    }
}
