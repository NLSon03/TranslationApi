using System.ComponentModel.DataAnnotations;

namespace TranslationApi.Application.DTOs
{
    public class UserListDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        // Thông tin cơ bản
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }

        // Thông tin ngôn ngữ và tùy chọn
        public string PreferredLanguage { get; set; } = "vi-VN";
        public List<string> FrequentlyUsedLanguages { get; set; } = new List<string>();
        public string Theme { get; set; } = "light";
        public DateTime LastActive { get; set; }
        public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";

        // Thống kê
        public int TranslationCount { get; set; }
        public DateTime MemberSince { get; set; }
    }

    public class UpdateUserDto
    {
        [Required]
        [StringLength(50)]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [StringLength(50)]
        public string? DisplayName { get; set; }

        [Url]
        public string? AvatarUrl { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UpdateUserPreferencesDto
    {
        [Required]
        public string PreferredLanguage { get; set; } = "vi-VN";

        public List<string> FrequentlyUsedLanguages { get; set; } = new List<string>();

        [Required]
        public string Theme { get; set; } = "light";

        [Required]
        public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";

        public bool EmailNotificationsEnabled { get; set; } = true;
    }

    public class ChangePasswordDto
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string NewPassword { get; set; }
    }

    public class ToggleLockoutDto
    {
        public required bool IsLocked { get; set; }
    }
}