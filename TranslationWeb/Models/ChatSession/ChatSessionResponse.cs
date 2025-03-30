using System.Text.Json.Serialization;
using TranslationWeb.Models.ChatMessage;

namespace TranslationWeb.Models.ChatSession
{
    public enum ChatSessionStatus
    {
        Active,
        Closed
    }

    public class ChatSessionResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("startedAt")]
        public DateTime StartedAt { get; set; }

        [JsonPropertyName("endedAt")]
        public DateTime? EndedAt { get; set; }

        [JsonPropertyName("status")]
        public ChatSessionStatus Status { get; set; }

        [JsonIgnore]
        public bool IsActive => Status == ChatSessionStatus.Active;

        [JsonPropertyName("aiModelId")]
        public Guid AIModelId { get; set; }

        [JsonPropertyName("aiModelName")]
        public string AIModelName { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<ChatMessageResponse> Messages { get; set; } = new List<ChatMessageResponse>();

        public override string ToString()
        {
            return $"Session[Id={Id}, AIModel={AIModelName}, Status={Status}, Messages={Messages.Count}]";
        }
    }
}