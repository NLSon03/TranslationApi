using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TranslationWeb.Infrastructure.Services;
using TranslationWeb.Models.Auth;

namespace TranslationWeb.Core.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly LocalStorageService _localStorage;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(LocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSession = await _localStorage.GetItemAsync<AuthResponse>("user_session");
                if (userSession == null)
                    return new AuthenticationState(_anonymous);

                var claimsPrincipal = CreateClaimsPrincipal(userSession);
                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task UpdateAuthenticationState(AuthResponse? userSession)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal;

                if (userSession != null && !string.IsNullOrEmpty(userSession.Token))
                {
                    // Lưu session mới
                    await _localStorage.SetItemAsync("user_session", userSession);

                    // Tạo claims principal mới
                    claimsPrincipal = CreateClaimsPrincipal(userSession);

                    // Kiểm tra xem claims principal có được tạo đúng không
                    if (claimsPrincipal.Identity?.IsAuthenticated != true)
                    {
                        // Nếu không authenticated, xóa session và sử dụng anonymous
                        await _localStorage.RemoveItemAsync("user_session");
                        claimsPrincipal = _anonymous;
                    }
                }
                else
                {
                    // Xóa session cũ và sử dụng anonymous
                    await _localStorage.RemoveItemAsync("user_session");
                    claimsPrincipal = _anonymous;
                }

                // Thông báo thay đổi trạng thái xác thực
                var authState = new AuthenticationState(claimsPrincipal);
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
            }
            catch
            {
                // Trong trường hợp lỗi, đảm bảo user được logout
                await _localStorage.RemoveItemAsync("user_session");
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            }
        }

        private ClaimsPrincipal CreateClaimsPrincipal(AuthResponse userSession)
        {
            try
            {
                // Kiểm tra token hết hạn
                if (string.IsNullOrEmpty(userSession.Token) || userSession.ExpiresAt < DateTime.Now)
                {
                    return _anonymous;
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.UserId),
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim(ClaimTypes.Authentication, "true")
                };

                // Đảm bảo Roles không null và thêm role claims
                if (userSession.Roles != null)
                {
                    foreach (var role in userSession.Roles)
                    {
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }
                }

                // Tạo identity với authentication type và name/role selectors
                var identity = new ClaimsIdentity(
                    claims,
                    "JWT Authentication",
                    ClaimTypes.Name,
                    ClaimTypes.Role);

                return new ClaimsPrincipal(identity);
            }
            catch
            {
                return _anonymous;
            }
        }
    }
}