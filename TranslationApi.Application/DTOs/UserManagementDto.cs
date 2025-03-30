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
    }

    public class UpdateUserDto
    {
        [Required]
        [StringLength(50)]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
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