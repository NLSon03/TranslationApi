using TranslationApi.Application.DTOs;

namespace TranslationApi.Application.Interfaces
{
    public interface IChatSessionService
    {
        Task<ChatSessionDto> CreateSessionAsync(string userId, Guid modelId);
        Task<ChatSessionDto?> GetSessionByIdAsync(Guid id);
        Task<ChatSessionDetailDto?> GetSessionWithMessagesAsync(Guid id);
        Task<IEnumerable<ChatSessionDto>> GetSessionsByUserIdAsync(string userId);
        Task<IEnumerable<ChatSessionDto>> GetAllSessionsAsync();
        Task<IEnumerable<ChatSessionDto>> GetSessionsByModelIdAsync(Guid modelId);
        Task<IEnumerable<ChatSessionDto>> GetActiveSessionsAsync();
        Task EndSessionAsync(Guid id);
        Task DeleteSessionAsync(Guid id);
        Task<bool> IsUserSessionOwnerAsync(Guid sessionId, string userId);
    }
}