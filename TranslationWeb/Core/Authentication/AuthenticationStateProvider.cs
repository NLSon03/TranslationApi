using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TranslationWeb.Infrastructure.Services;
using TranslationWeb.Models.Auth;

namespace TranslationWeb.Core.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly LocalStorageService _localStorage;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(
            LocalStorageService localStorage,
            ILogger<CustomAuthenticationStateProvider> logger)
        {
            _localStorage = localStorage;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                _logger.LogDebug("Đang lấy authentication state");
                
                var userSession = await _localStorage.GetItemAsync<AuthResponse>("user_session");
                if (userSession == null)
                {
                    _logger.LogInformation("Không tìm thấy session, trả về anonymous state");
                    return new AuthenticationState(_anonymous);
                }

                // Kiểm tra token expiration
                if (userSession.IsAccessTokenExpired())
                {
                    _logger.LogWarning("Token đã hết hạn");
                    await _localStorage.RemoveItemAsync("user_session");
                    return new AuthenticationState(_anonymous);
                }

                var claimsPrincipal = CreateClaimsPrincipal(userSession);
                if (claimsPrincipal.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInformation("Authentication state được tạo thành công cho user: {Username}", 
                        claimsPrincipal.Identity.Name);
                    return new AuthenticationState(claimsPrincipal);
                }

                _logger.LogWarning("Không thể tạo authenticated principal, trả về anonymous state");
                return new AuthenticationState(_anonymous);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy authentication state");
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task UpdateAuthenticationState(AuthResponse? userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            try
            {
                if (userSession != null && !string.IsNullOrEmpty(userSession.Token) && !userSession.IsAccessTokenExpired())
                {
                    _logger.LogInformation("Đang cập nhật authentication state cho user: {Username}", 
                        userSession.UserName);

                    // Lưu session mới
                    await _localStorage.SetItemAsync("user_session", userSession);

                    // Tạo claims principal mới
                    claimsPrincipal = CreateClaimsPrincipal(userSession);

                    // Kiểm tra xem claims principal có được tạo đúng không
                    if (claimsPrincipal.Identity?.IsAuthenticated != true)
                    {
                        _logger.LogWarning("Claims principal không hợp lệ, chuyển về anonymous");
                        await _localStorage.RemoveItemAsync("user_session");
                        claimsPrincipal = _anonymous;
                    }
                }
                else
                {
                    _logger.LogInformation("Xóa session và chuyển về anonymous state");
                    await _localStorage.RemoveItemAsync("user_session");
                    claimsPrincipal = _anonymous;
                }

                // Thông báo thay đổi trạng thái xác thực
                var authState = new AuthenticationState(claimsPrincipal);
                _logger.LogInformation("Đang thông báo thay đổi trạng thái xác thực. IsAuthenticated: {IsAuthenticated}", 
                    claimsPrincipal.Identity?.IsAuthenticated);
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
                
                // Đợi một chút để đảm bảo các component đã nhận được thông báo
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật authentication state");
                
                // Trong trường hợp lỗi, đảm bảo user được logout
                await _localStorage.RemoveItemAsync("user_session");
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            }
        }

        private ClaimsPrincipal CreateClaimsPrincipal(AuthResponse userSession)
        {
            try
            {
                if (string.IsNullOrEmpty(userSession.Token) || 
                    string.IsNullOrEmpty(userSession.UserId) || 
                    string.IsNullOrEmpty(userSession.UserName))
                {
                    _logger.LogWarning("Thông tin user session không hợp lệ");
                    return _anonymous;
                }

                // Kiểm tra token hết hạn
                if (userSession.IsAccessTokenExpired())
                {
                    _logger.LogWarning("Token đã hết hạn khi tạo claims principal");
                    return _anonymous;
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.UserId),
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim("AccessToken", userSession.Token),
                    new Claim("TokenExpiration", userSession.ExpiresAt.ToString("O")),
                    new Claim(ClaimTypes.Authentication, "true")
                };

                // Thêm refresh token claim nếu có
                if (!string.IsNullOrEmpty(userSession.RefreshToken))
                {
                    claims.Add(new Claim("RefreshToken", userSession.RefreshToken));
                    claims.Add(new Claim("RefreshTokenExpiration", 
                        userSession.RefreshTokenExpiresAt.ToString("O")));
                }

                // Đảm bảo Roles không null và thêm role claims
                if (userSession.Roles != null)
                {
                    foreach (var role in userSession.Roles.Where(r => !string.IsNullOrWhiteSpace(r)))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                // Tạo identity với authentication type và name/role selectors
                var identity = new ClaimsIdentity(
                    claims,
                    "JWT Authentication",
                    ClaimTypes.Name,
                    ClaimTypes.Role);

                _logger.LogInformation("Claims principal được tạo thành công cho user: {Username}", 
                    userSession.UserName);

                return new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo claims principal");
                return _anonymous;
            }
        }
    }
}