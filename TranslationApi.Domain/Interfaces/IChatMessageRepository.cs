using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Enums;

namespace TranslationApi.Domain.Interfaces
{
    public interface IChatMessageRepository : IRepository<ChatMessage>
    {
        Task<IEnumerable<ChatMessage>> GetMessagesBySessionIdAsync(Guid sessionId);
        Task<IEnumerable<ChatMessage>> GetMessagesBySenderTypeAsync(Guid sessionId, SenderType senderType);
        Task<IEnumerable<ChatMessage>> GetMessagesByMessageTypeAsync(Guid sessionId, MessageType messageType);
        Task<double> GetAverageResponseTimeAsync(Guid sessionId);
        Task<ChatMessage?> GetLastMessageInSessionAsync(Guid sessionId);
    }
} 