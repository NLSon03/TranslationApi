using Microsoft.AspNetCore.Components.Authorization;
using TranslationWeb.Core.Authentication;
using TranslationWeb.Core.Constants;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.Auth;

namespace TranslationWeb.Infrastructure.Services
{
    public class AuthService : IAuthService, IDisposable
    {
        private readonly HttpService _httpService;
        private readonly TokenService _tokenService;
        private readonly LocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<AuthService> _logger;

        public event Action? OnSessionTimeout;

        public AuthService(
            HttpService httpService,
            TokenService tokenService,
            LocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider,
            ILogger<AuthService> logger)
        {
            _httpService = httpService;
            _tokenService = tokenService;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _logger = logger;

            // Đăng ký xử lý session timeout
            _tokenService.OnSessionTimeout += HandleSessionTimeout;
        }

        private void HandleSessionTimeout()
        {
            OnSessionTimeout?.Invoke();
            LogoutAsync().ConfigureAwait(false);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation("Đang thực hiện đăng nhập cho user: {Username}", loginRequest.Email);

                var response = await _httpService.PostAsync<LoginRequest, AuthResponse>(
                    ApiEndpoints.Auth.Login,
                    loginRequest
                );

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    if (response.ExpiresAt == default)
                    {
                        response.ExpiresAt = DateTime.Now.AddHours(1); // Access token 1 giờ
                        response.RefreshTokenExpiresAt = DateTime.Now.AddDays(7); // Refresh token 7 ngày
                    }

                    response.Success = true;
                    response.RememberMe = loginRequest.RememberMe;

                    // Lưu token và cập nhật state
                    await _tokenService.StoreToken(response);
                    await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(response);

                    _logger.LogInformation("Đăng nhập thành công cho user: {Username}", loginRequest.Email);
                }
                else
                {
                    _logger.LogWarning("Đăng nhập thất bại cho user: {Username}", loginRequest.Email);
                }

                return response ?? new AuthResponse { Success = false, Message = "Đăng nhập thất bại" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập cho user: {Username}", loginRequest.Email);
                return new AuthResponse { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                _logger.LogInformation("Đang thực hiện đăng ký cho user: {Username}", registerRequest.Email);

                var response = await _httpService.PostAsync<RegisterRequest, AuthResponse>(
                    ApiEndpoints.Auth.Register,
                    registerRequest
                );

                if (response != null)
                {
                    if (!string.IsNullOrEmpty(response.Token))
                    {
                        response.Success = true;
                        response.RememberMe = true; // Mặc định remember me khi đăng ký

                        // Lưu token và cập nhật state
                        await _tokenService.StoreToken(response);
                        await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(response);

                        _logger.LogInformation("Đăng ký thành công cho user: {Username}", registerRequest.Email);
                    }
                    else
                    {
                        _logger.LogWarning("Đăng ký thất bại cho user: {Username}", registerRequest.Email);
                    }
                }

                return response ?? new AuthResponse { Success = false, Message = "Đăng ký thất bại" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký cho user: {Username}", registerRequest.Email);
                return new AuthResponse { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> GetCurrentUserAsync()
        {
            try
            {
                // Validate token hiện tại
                if (!await _tokenService.ValidateCurrentToken())
                {
                    _logger.LogWarning("Token không hợp lệ khi lấy thông tin user hiện tại");
                    return new AuthResponse { Success = false, Message = "Phiên đăng nhập đã hết hạn" };
                }

                var response = await _httpService.GetAsync<AuthResponse>(ApiEndpoints.Auth.CurrentUser);
                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    // Cập nhật token mới nếu server trả về
                    await _tokenService.StoreToken(response);
                    await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(response);
                    
                    _logger.LogInformation("Lấy thông tin user hiện tại thành công: {Username}", response.UserName);
                    return response;
                }

                _logger.LogWarning("Không thể lấy thông tin user hiện tại");
                return new AuthResponse { Success = false, Message = "Không thể lấy thông tin người dùng" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin user hiện tại");
                return new AuthResponse { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _logger.LogInformation("Đang thực hiện đăng xuất");

                // Xóa token và cập nhật state
                await _tokenService.ClearToken();
                await ((CustomAuthenticationStateProvider)_authStateProvider).UpdateAuthenticationState(null);

                _logger.LogInformation("Đăng xuất thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng xuất");
                throw;
            }
        }

        public void UpdateLastActivity()
        {
            _tokenService.UpdateLastActivity();
        }

        public void Dispose()
        {
            _tokenService.OnSessionTimeout -= HandleSessionTimeout;
        }
    }
}