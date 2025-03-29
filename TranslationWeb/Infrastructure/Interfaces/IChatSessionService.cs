using TranslationWeb.Models.ChatSession;

namespace TranslationWeb.Infrastructure.Interfaces
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