namespace TranslationApi.Application.DTOs
{
    public class ChatSessionDto
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public ChatSessionStatus Status { get; set; }
        public string? AIModelName { get; set; }
    }

    public class ChatSessionDetailDto : ChatSessionDto
    {
        public ICollection<ChatMessageDto>? Messages { get; set; }
    }

}