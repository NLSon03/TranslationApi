
using TranslationApi.Application.DTOs;
using TranslationApi.Domain.Entities;

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
    }
}