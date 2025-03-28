using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Application.Interfaces
{
    public interface IChatSessionService
    {
        Task<ChatSession> CreateSessionAsync(string userId, Guid modelId);
        Task<ChatSession?> GetSessionByIdAsync(Guid id);
        Task<ChatSession?> GetSessionWithMessagesAsync(Guid id);
        Task<IEnumerable<ChatSession>> GetSessionsByUserIdAsync(string userId);
        Task<IEnumerable<ChatSession>> GetAllSessionsAsync();
        Task<IEnumerable<ChatSession>> GetSessionsByModelIdAsync(Guid modelId);
        Task<IEnumerable<ChatSession>> GetActiveSessionsAsync();
        Task EndSessionAsync(Guid id);
        Task DeleteSessionAsync(Guid id);
        Task<bool> IsUserSessionOwnerAsync(Guid sessionId, string userId);
    }
} 