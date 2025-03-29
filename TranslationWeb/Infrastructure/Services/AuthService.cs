using Microsoft.AspNetCore.Components.Authorization;
using TranslationWeb.Core.Authentication;
using TranslationWeb.Core.Constants;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.Auth;

namespace TranslationWeb.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpService _httpService;
        private readonly LocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            HttpService httpService,
            LocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _httpService = httpService;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var response = await _httpService.PostAsync<AuthResponse>(ApiEndpoints.Auth.Login, loginRequest);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    if (response.ExpiresAt == default)
                    {
                        response.ExpiresAt = DateTime.Now.AddHours(24);
                    }

                    response.Success = true;

                    await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(response);
                }

                return response ?? new AuthResponse { Success = false, Message = "Đăng nhập thất bại" };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                var response = await _httpService.PostAsync<AuthResponse>(ApiEndpoints.Auth.Register, registerRequest);

                if (response != null)
                {
                    if (!string.IsNullOrEmpty(response.Token))
                    {
                        response.Success = true;
                    }

                    if (response.Success)
                    {
                        await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(response);
                    }
                }

                return response ?? new AuthResponse { Success = false, Message = "Đăng ký thất bại" };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> GetCurrentUserAsync()
        {
            try
            {
                var savedToken = await _localStorage.GetItemAsync<AuthResponse>("user_session");
                if (savedToken == null)
                {
                    await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(null);
                    return new AuthResponse { Success = false, Message = "Người dùng chưa đăng nhập" };
                }

                // Add token to request
                var response = await _httpService.GetAsync<AuthResponse>(ApiEndpoints.Auth.CurrentUser);
                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    // Cập nhật AuthenticationState với thông tin mới nhất
                    await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(response);
                    return response;
                }
                
                // Nếu không lấy được thông tin mới, xóa session cũ
                await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(null);
                return new AuthResponse { Success = false, Message = "Không thể lấy thông tin người dùng" };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task LogoutAsync()
        {
            await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(null);
        }
    }
}