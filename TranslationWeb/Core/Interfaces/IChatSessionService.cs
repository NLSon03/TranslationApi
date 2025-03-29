using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationWeb.Models.ChatSession;

namespace TranslationWeb.Core.Interfaces
{
    public interface IChatSessionService
    {
        Task<ChatSessionResponse> CreateSessionAsync(CreateSessionRequest request);
        Task<ChatSessionResponse> GetSessionByIdAsync(Guid sessionId);
        Task<IEnumerable<ChatSessionResponse>> GetUserSessionsAsync(string userId);
        Task<ChatSessionResponse> EndSessionAsync(Guid sessionId);
        Task<bool> DeleteSessionAsync(Guid sessionId);
    }
}