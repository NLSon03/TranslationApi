using System.ComponentModel.DataAnnotations;
using TranslationApi.Domain.Enums;

namespace TranslationApi.Domain.Entities
{
    public class Feedback
    {
        public Guid Id { get; init; }
        public Guid MessageId { get; set; }
        public ChatMessage Message { get; set; }
        public FeedbackRating Rating { get; set; }
        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
