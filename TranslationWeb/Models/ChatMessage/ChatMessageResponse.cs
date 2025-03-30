using System.Text.Json.Serialization;

namespace TranslationWeb.Models.ChatMessage
{
    public enum SenderType
    {
        User,
        AI
    }

    public class ChatMessageResponse
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Content { get; set; } = string.Empty;
        
        public SenderType SenderType { get; set; }
        
        public MessageType MessageType { get; set; }
        
        public DateTime SentAt { get; set; }
        public long? ResponseTime { get; set; }
        public bool HasFeedback { get; set; }
    }
}