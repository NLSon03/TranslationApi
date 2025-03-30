
using System;
using System.Collections.Generic;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;

namespace TranslationApi.Application.DTOs
{
    public class ChatSessionDto
    {
        public Guid Id { get; set; }

        public string? UserId {  get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public ChatSessionStatus Status { get; set; }
        public string? AIModelName { get; set; }
    }

    public class ChatSessionDetailDto : ChatSessionDto
    {
        public ICollection<ChatMessageDto>? Messages { get; set; }
    }

    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public MessageType Type { get; set; }
        public SenderType SenderType { get; set; }
    }
}