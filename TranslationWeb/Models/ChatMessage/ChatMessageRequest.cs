using System.ComponentModel.DataAnnotations;

namespace TranslationWeb.Models.ChatMessage
{
    public class SendMessageRequest
    {
        public Guid SessionId { get; set; }

        [Required(ErrorMessage = "Nội dung tin nhắn là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        public string MessageType { get; set; } = "Text";

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