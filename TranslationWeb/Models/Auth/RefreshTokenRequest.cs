using System.Text.Json.Serialization;

namespace TranslationWeb.Models.Auth
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}