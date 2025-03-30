using TranslationApi.Domain.Enums;

namespace TranslationApi.Application.DTOs
{
    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Content { get; set; }
        public SenderType SenderType { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime SentAt { get; set; }
        public long? ResponseTime { get; set; }
    }
}