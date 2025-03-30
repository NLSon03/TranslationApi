using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TranslationWeb.Models.ChatMessage
{
    public class SendMessageRequest
    {
        [Required]
        public Guid SessionId { get; set; }

        [Required(ErrorMessage = "Nội dung tin nhắn là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageType MessageType { get; set; } = MessageType.Text;

        public string FromLanguage { get; set; } = "auto";

        public string ToLanguage { get; set; } = "vi";
    }

    public class UpdateMessageRequest
    {
        public Guid MessageId { get; set; }

        [Required(ErrorMessage = "Nội dung tin nhắn là bắt buộc")]
        public string Content { get; set; } = string.Empty;
    }
}