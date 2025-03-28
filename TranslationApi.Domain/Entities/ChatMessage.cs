using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TranslationApi.Domain.Enums;

namespace TranslationApi.Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; init; }
        public Guid SessionId { get; set; }
        public ChatSession Session { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
        public SenderType SenderType { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public long? ResponseTime { get; set; } // milliseconds
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
