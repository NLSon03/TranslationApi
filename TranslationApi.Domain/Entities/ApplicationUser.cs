using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationApi.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required ICollection<ChatSession> ChatSessions { get; set; }
    }
}
