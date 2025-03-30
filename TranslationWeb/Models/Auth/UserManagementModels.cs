using System.ComponentModel.DataAnnotations;

namespace TranslationWeb.Models.Auth
{
    public class UserListResponse
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên người dùng không được vượt quá 50 ký tự")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public required string NewPassword { get; set; }
    }

    public class ToggleLockoutRequest
    {
        public required bool IsLocked { get; set; }
    }
}