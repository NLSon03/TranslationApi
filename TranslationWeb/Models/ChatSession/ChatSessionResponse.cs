using TranslationWeb.Models.ChatMessage;

namespace TranslationWeb.Models.ChatSession
{
    public class ChatSessionResponse
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid AIModelId { get; set; }
        public string AIModelName { get; set; } = string.Empty;
        public List<ChatMessageResponse> Messages { get; set; } = new List<ChatMessageResponse>();
    }
}