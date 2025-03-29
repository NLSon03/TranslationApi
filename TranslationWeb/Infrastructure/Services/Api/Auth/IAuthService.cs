using TranslationWeb.Models.Auth;

namespace TranslationWeb.Infrastructure.Services.Api.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<AuthResponse> GetCurrentUserAsync();
        Task LogoutAsync();
    }
} 