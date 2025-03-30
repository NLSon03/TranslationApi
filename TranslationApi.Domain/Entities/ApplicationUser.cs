using Microsoft.AspNetCore.Identity;

namespace TranslationApi.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required ICollection<ChatSession> ChatSessions { get; set; }
    }
}
