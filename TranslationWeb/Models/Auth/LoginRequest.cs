using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TranslationWeb.Models.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}