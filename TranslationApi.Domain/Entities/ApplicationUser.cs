using Microsoft.AspNetCore.Identity;

namespace TranslationApi.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            ChatSessions = new List<ChatSession>();
            FrequentlyUsedLanguages = new List<string>();
            MemberSince = DateTime.UtcNow;
            LastActive = DateTime.UtcNow;
            Theme = "light";
            PreferredLanguage = "vi-VN";
            EmailNotificationsEnabled = true;
            TranslationCount = 0;
            TimeZone = "Asia/Ho_Chi_Minh";
        }

        // Thông tin cơ bản
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }

        // Cài đặt ngôn ngữ
        public string PreferredLanguage { get; set; }
        public List<string> FrequentlyUsedLanguages { get; set; }

        // Tùy chọn người dùng
        public bool EmailNotificationsEnabled { get; set; }
        public string Theme { get; set; }
        public DateTime LastActive { get; set; }
        public string TimeZone { get; set; }

        // Thống kê sử dụng
        public int TranslationCount { get; set; }
        public DateTime MemberSince { get; set; }

        // Quan hệ với các entity khác
        public required ICollection<ChatSession> ChatSessions { get; set; }
    }
}