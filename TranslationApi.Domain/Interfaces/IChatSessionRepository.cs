using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Domain.Interfaces
{
    public interface IChatSessionRepository : IRepository<ChatSession>
    {
        Task<IEnumerable<ChatSession>> GetSessionsByUserIdAsync(string userId);
        Task<ChatSession?> GetSessionWithMessagesAsync(Guid sessionId);
        Task<IEnumerable<ChatSession>> GetSessionsByModelIdAsync(Guid modelId);
        Task<IEnumerable<ChatSession>> GetActiveSessions();
        Task EndSessionAsync(Guid sessionId);
    }
} 