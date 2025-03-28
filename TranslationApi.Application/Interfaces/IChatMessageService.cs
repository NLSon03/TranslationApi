using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;

namespace TranslationApi.Application.Interfaces
{
    public interface IChatMessageService
    {
        Task<ChatMessage> AddMessageAsync(Guid sessionId, string content, SenderType senderType, MessageType messageType, long? responseTime = null);
        Task<ChatMessage?> GetMessageByIdAsync(Guid id);
        Task<IEnumerable<ChatMessage>> GetMessagesBySessionIdAsync(Guid sessionId);
        Task<IEnumerable<ChatMessage>> GetMessagesBySenderTypeAsync(Guid sessionId, SenderType senderType);
        Task<IEnumerable<ChatMessage>> GetMessagesByMessageTypeAsync(Guid sessionId, MessageType messageType);
        Task<double> GetAverageResponseTimeAsync(Guid sessionId);
        Task<ChatMessage?> GetLastMessageInSessionAsync(Guid sessionId);
        Task DeleteMessageAsync(Guid id);
        Task UpdateMessageContentAsync(Guid id, string newContent);
    }
} 