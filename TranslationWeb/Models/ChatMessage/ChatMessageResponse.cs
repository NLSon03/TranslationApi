namespace TranslationWeb.Models.ChatMessage
{
    public class ChatMessageResponse
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string SenderType { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public long? ResponseTime { get; set; }
        public bool HasFeedback { get; set; }
    }
}