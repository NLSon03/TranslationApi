using TranslationWeb.Core.Constants;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.Auth;

namespace TranslationWeb.Infrastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly HttpService _httpClient;

        public UserManagementService(HttpService httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserListResponse>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync<List<UserListResponse>>(ApiEndpoints.Auth.Users.GetAll);
            return response ?? new List<UserListResponse>();
        }

        public async Task<UserListResponse> GetUserAsync(string id)
        {
            var response = await _httpClient.GetAsync<UserListResponse>($"{ApiEndpoints.Auth.Users.GetById}/{id}");
            if (response == null)
            {
                throw new Exception("Không thể tải thông tin người dùng");
            }
            return response;
        }

        public async Task UpdateUserAsync(string id, UpdateUserRequest request)
        {
            try
            {
                await _httpClient.PutAsync<UpdateUserRequest, object>($"{ApiEndpoints.Auth.Users.Update}/{id}", request);
            }
            catch (HttpRequestException)
            {
                throw new Exception("Không thể cập nhật thông tin người dùng");
            }
        }

        public async Task ResetPasswordAsync(string id, ChangePasswordRequest request)
        {
            try
            {
                await _httpClient.PostAsync<ChangePasswordRequest, object>($"{ApiEndpoints.Auth.Users.ResetPassword}/{id}", request);
            }
            catch (HttpRequestException)
            {
                throw new Exception("Không thể đặt lại mật khẩu");
            }
        }

        public async Task ToggleLockoutAsync(string id, ToggleLockoutRequest request)
        {
            try
            {
                await _httpClient.PostAsync<ToggleLockoutRequest, object>($"{ApiEndpoints.Auth.Users.ToggleLockout}/{id}", request);
            }
            catch (HttpRequestException)
            {
                throw new Exception("Không thể thay đổi trạng thái khóa tài khoản");
            }
        }
    }
}