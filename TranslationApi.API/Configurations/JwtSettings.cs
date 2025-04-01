using System.ComponentModel.DataAnnotations;

namespace TranslationApi.API.Configurations
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        [Required(ErrorMessage = "Issuer is required")]
        public required string Issuer { get; set; }
        [Required(ErrorMessage = "Audience is required")]
        public required string Audience { get; set; }
        [Required(ErrorMessage = "SecretKey is required")]
        public required string SecretKey { get; set; }
        public int TokenExpirationMinutes { get; set; } = 60;
    }
}
