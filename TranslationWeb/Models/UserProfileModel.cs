using System;
using System.Collections.Generic;

namespace TranslationWeb.Models
{
    public class UserProfileModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public List<string> Roles { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public string PreferredLanguage { get; set; }
        public List<string> FrequentlyUsedLanguages { get; set; }
        public string Theme { get; set; }
        public DateTime LastActive { get; set; }
        public string TimeZone { get; set; }
        public int TranslationCount { get; set; }
        public DateTime MemberSince { get; set; }
    }
}
