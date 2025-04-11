using Microsoft.AspNetCore.Identity;
using TranslationApi.Application.DTOs;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserListDto> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Không tìm thấy người dùng.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserListDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                Roles = roles.ToList(),
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio,
                PreferredLanguage = user.PreferredLanguage,
                FrequentlyUsedLanguages = user.FrequentlyUsedLanguages,
                Theme = user.Theme,
                LastActive = user.LastActive,
                TimeZone = user.TimeZone,
                TranslationCount = user.TranslationCount,
                MemberSince = user.MemberSince
            };
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.DisplayName = updateUserDto.DisplayName;
            user.AvatarUrl = updateUserDto.AvatarUrl;
            user.Bio = updateUserDto.Bio;
            user.UserName = updateUserDto.UserName;
            user.Email = updateUserDto.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Cập nhật roles nếu có thay đổi
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Except(updateUserDto.Roles);
                var rolesToAdd = updateUserDto.Roles.Except(currentRoles);

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }

                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
                }

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUserPreferencesAsync(string userId, UpdateUserPreferencesDto preferencesDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.PreferredLanguage = preferencesDto.PreferredLanguage;
            user.FrequentlyUsedLanguages = preferencesDto.FrequentlyUsedLanguages;
            user.Theme = preferencesDto.Theme;
            user.TimeZone = preferencesDto.TimeZone;
            user.EmailNotificationsEnabled = preferencesDto.EmailNotificationsEnabled;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> IncrementTranslationCountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.TranslationCount++;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateLastActiveAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.LastActive = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<List<string>> GetFrequentlyUsedLanguagesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new List<string>();
            }

            return user.FrequentlyUsedLanguages;
        }
    }
}