
using TranslationApi.Application.DTOs;

namespace TranslationApi.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserListDto> GetUserProfileAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, UpdateUserDto updateUserDto);
        Task<bool> UpdateUserPreferencesAsync(string userId, UpdateUserPreferencesDto preferencesDto);
        Task<bool> IncrementTranslationCountAsync(string userId);
        Task<bool> UpdateLastActiveAsync(string userId);
        Task<List<string>> GetFrequentlyUsedLanguagesAsync(string userId);
        Task<bool> UpdateUserAvatarAsync(string userId, string avatarUrl);
        
    }
}