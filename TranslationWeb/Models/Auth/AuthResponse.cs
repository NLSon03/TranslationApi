using System.Text.Json.Serialization;

namespace TranslationWeb.Models.Auth
{
    public class AuthResponse
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("expiration")]
        public DateTime ExpiresAt { get; set; }

        [JsonPropertyName("refreshTokenExpiration")]
        public DateTime RefreshTokenExpiresAt { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; } = new List<string>();

        [JsonIgnore]
        public bool IsAdmin => Roles.Contains("Admin");

        [JsonPropertyName("rememberMe")]
        public bool RememberMe { get; set; }

        [JsonPropertyName("lastActivityAt")]
        public DateTime LastActivityAt { get; set; }

        public bool IsAccessTokenExpired() => DateTime.Now >= ExpiresAt;

        public bool IsRefreshTokenExpired() => DateTime.Now >= RefreshTokenExpiresAt;

        public bool ShouldRefreshToken() =>
            DateTime.Now.AddMinutes(5) >= ExpiresAt && !IsRefreshTokenExpired();
    }
}