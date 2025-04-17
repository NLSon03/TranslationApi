using TranslationWeb.Core.Constants;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models;
using TranslationWeb.Models.Translation;

namespace TranslationWeb.Infrastructure.Services
{
    public class UserProfileService : IUserProfileService
    {
        public class AvatarUploadResponse
        {
            public string? avatarUrl { get; set; }
        }
        private readonly HttpService _httpService;
        public UserProfileService(HttpService httpService)
        {
            _httpService = httpService;
        }



        public async Task<UserProfileModel?> GetCurrentUserProfileAsync()
        {
            var endpoint = ApiEndpoints.UserProfile.Profile;
            try
            {
                return await _httpService.GetAsync<UserProfileModel>(endpoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy hồ sơ user: " + ex.Message);
                return null;
            }
        }

        public async Task<List<Language>> GetFrequentlyUsedLanguagesAsync()
        {
            var endpoint = ApiEndpoints.UserProfile.FrequentlyUsedLanguages;
            try
            {
                var result = await _httpService.GetAsync<List<Language>>(endpoint);
                return result ?? new List<Language>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy danh sách ngôn ngữ thường dùng: " + ex.Message);
                return new List<Language>();
            }
        }

        public async Task UpdateProfileAsync(UserProfileModel profile)
        {
            var endpoint = ApiEndpoints.UserProfile.UpdateProfile;
            try
            {
                await _httpService.PutAsync<UserProfileModel, UserProfileModel>(endpoint, profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật hồ sơ user: " + ex.Message);
            }
        }

        public async Task UpdatePreferencesAsync(UserProfileModel profile)
        {
            var endpoint = ApiEndpoints.UserProfile.UpdatePreferences;
            try
            {
                await _httpService.PutAsync<UserProfileModel, UserProfileModel>(endpoint, profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật preferences: " + ex.Message);
            }
        }

        public async Task UpdateLastActiveAsync(UserProfileModel profile)
        {
            var endpoint = ApiEndpoints.UserProfile.UpdateLastActive;
            try
            {
                await _httpService.PutAsync<UserProfileModel, UserProfileModel>(endpoint, profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật hoạt động gần đây: " + ex.Message);
            }
        }

        public async Task IncrementTranslationCountAsync(UserProfileModel profile)
        {
            var endpoint = ApiEndpoints.UserProfile.IncrementTranslationCount;
            try
            {
                await _httpService.PostAsync<UserProfileModel, UserProfileModel>(endpoint, profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi tăng số lần dịch: " + ex.Message);
            }
        }

        public async Task UpdateFrequentlyUsedLanguagesAsync(UserProfileModel profile)
        {
            var endpoint = ApiEndpoints.UserProfile.FrequentlyUsedLanguages;
            try
            {
                await _httpService.PostAsync<UserProfileModel, UserProfileModel>(endpoint, profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật ngôn ngữ thường dùng: " + ex.Message);
            }
        }
        public async Task<string?> UploadAvatarAsync(Stream fileStream, string fileName)
        {
            var endpoint = ApiEndpoints.UserProfile.UploadAvatar;
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(fileStream), "file", fileName);
                var result = await _httpService.PostMultipartAsync<AvatarUploadResponse>(endpoint, content);
                return result?.avatarUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi upload avatar: " + ex.Message);
                return null;
            }
        }
    }
}