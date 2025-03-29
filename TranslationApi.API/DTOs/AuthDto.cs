using System.ComponentModel.DataAnnotations;

namespace TranslationApi.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }

        [Required]
        [StringLength(50)]
        public required string UserName { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
} 