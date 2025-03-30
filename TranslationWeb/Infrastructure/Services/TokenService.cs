using Microsoft.AspNetCore.Components.Authorization;
using System.Timers;
using TranslationWeb.Core.Authentication;
using TranslationWeb.Core.Constants;
using TranslationWeb.Models.Auth;

namespace TranslationWeb.Infrastructure.Services
{
    public class TokenService : IDisposable
    {
        private readonly HttpService _httpService;
        private readonly LocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<TokenService> _logger;
        private System.Timers.Timer? _tokenRefreshTimer;
        private System.Timers.Timer? _sessionTimeoutTimer;
        private const int TOKEN_REFRESH_INTERVAL = 4 * 60 * 1000; // 4 phút
        private const int SESSION_TIMEOUT = 30 * 60 * 1000; // 30 phút
        private DateTime _lastActivity;

        public event Action? OnSessionTimeout;

        public TokenService(
            HttpService httpService,
            LocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider,
            ILogger<TokenService> logger)
        {
            _httpService = httpService;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _logger = logger;
            _lastActivity = DateTime.Now;

            InitializeTimers();
        }

        private void InitializeTimers()
        {
            // Timer kiểm tra và refresh token
            _tokenRefreshTimer = new System.Timers.Timer(TOKEN_REFRESH_INTERVAL);
            _tokenRefreshTimer.Elapsed += async (sender, e) => await CheckAndRefreshToken();
            _tokenRefreshTimer.Start();

            // Timer kiểm tra session timeout
            _sessionTimeoutTimer = new System.Timers.Timer(60000); // Kiểm tra mỗi phút
            _sessionTimeoutTimer.Elapsed += CheckSessionTimeout;
            _sessionTimeoutTimer.Start();
        }

        public void UpdateLastActivity()
        {
            _lastActivity = DateTime.Now;
        }

        private void CheckSessionTimeout(object? sender, ElapsedEventArgs e)
        {
            var timeSinceLastActivity = DateTime.Now - _lastActivity;
            if (timeSinceLastActivity.TotalMilliseconds >= SESSION_TIMEOUT)
            {
                OnSessionTimeout?.Invoke();
                _sessionTimeoutTimer?.Stop();
            }
        }

        private async Task CheckAndRefreshToken()
        {
            try
            {
                var authResponse = await _localStorage.GetItemAsync<AuthResponse>("user_session");
                if (authResponse == null) return;

                if (authResponse.ShouldRefreshToken())
                {
                    await RefreshToken(authResponse.RefreshToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra và refresh token");
            }
        }

        public async Task<bool> RefreshToken(string refreshToken)
        {
            try
            {
                var response = await _httpService.PostAsync<RefreshTokenRequest, AuthResponse>(
                    ApiEndpoints.Auth.RefreshToken,
                    new RefreshTokenRequest { RefreshToken = refreshToken }
                );

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    await ((CustomAuthenticationStateProvider)_authStateProvider)
                        .UpdateAuthenticationState(response);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi refresh token");
            }

            return false;
        }

        public async Task<bool> ValidateCurrentToken()
        {
            try
            {
                var authResponse = await _localStorage.GetItemAsync<AuthResponse>("user_session");
                if (authResponse == null) return false;

                if (authResponse.IsAccessTokenExpired())
                {
                    if (authResponse.IsRefreshTokenExpired())
                    {
                        await ((CustomAuthenticationStateProvider)_authStateProvider)
                            .UpdateAuthenticationState(null);
                        return false;
                    }

                    return await RefreshToken(authResponse.RefreshToken);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi validate token");
                return false;
            }
        }

        public async Task StoreToken(AuthResponse authResponse)
        {
            try
            {
                if (authResponse.RememberMe)
                {
                    // Nếu remember me, lưu refresh token
                    await _localStorage.SetItemAsync("user_session", authResponse);
                }
                else
                {
                    // Nếu không remember me, chỉ lưu access token
                    authResponse.RefreshToken = string.Empty;
                    await _localStorage.SetItemAsync("user_session", authResponse);
                }

                // Reset session timeout
                _lastActivity = DateTime.Now;
                _sessionTimeoutTimer?.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu token");
            }
        }

        public async Task ClearToken()
        {
            try
            {
                await _localStorage.RemoveItemAsync("user_session");
                _sessionTimeoutTimer?.Stop();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa token");
            }
        }

        public void Dispose()
        {
            _tokenRefreshTimer?.Dispose();
            _sessionTimeoutTimer?.Dispose();
        }
    }
}