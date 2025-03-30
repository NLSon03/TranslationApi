using System.ComponentModel.DataAnnotations;

namespace TranslationApi.Domain.Entities
{
    public class AIModel
    {
        public Guid Id { get; init; }
        [Required, MaxLength(50)]
        public required string Name { get; set; }
        [Required, MaxLength(20)]
        public required string Version { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        public required string Config { get; set; }
        public required ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    }
}
