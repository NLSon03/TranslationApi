using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using TranslationWeb.Infrastructure.Services.LocalStorage;
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
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                await _localStorage.SetItemAsync("user_session", userSession);
                claimsPrincipal = CreateClaimsPrincipal(userSession);
            }
            else
            {
                await _localStorage.RemoveItemAsync("user_session");
                claimsPrincipal = _anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private ClaimsPrincipal CreateClaimsPrincipal(AuthResponse userSession)
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
                // Thêm claim xác thực cần thiết
                new Claim(ClaimTypes.Authentication, "true")
            };

            var identity = new ClaimsIdentity(claims, "JWT Authentication", ClaimTypes.Name, ClaimTypes.Role);
            return new ClaimsPrincipal(identity);
        }
    }
} 