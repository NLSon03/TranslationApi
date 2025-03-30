using System.ComponentModel.DataAnnotations;

namespace TranslationApi.Domain.Entities
{
    public class ChatSession
    {
        public Guid Id { get; init; }
        [Required, StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public ChatSessionStatus Status { get; set; }
        public Guid AIModelId { get; set; }
        public required AIModel AIModel { get; set; }
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
