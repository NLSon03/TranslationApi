using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TranslationWeb.Models.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("rememberMe")]
        public bool RememberMe { get; set; }
    }
}