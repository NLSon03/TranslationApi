using TranslationApi.Application.DTOs;
using TranslationApi.Domain.Enums;

namespace TranslationApi.Application.Interfaces
{
    public interface IChatMessageService
    {
        Task<ChatMessageDto> AddMessageAsync(Guid sessionId, string content, SenderType senderType, MessageType messageType, long? responseTime = null);
        Task<ChatMessageDto?> GetMessageByIdAsync(Guid id);
        Task<IEnumerable<ChatMessageDto>> GetMessagesBySessionIdAsync(Guid sessionId);
        Task<IEnumerable<ChatMessageDto>> GetMessagesBySenderTypeAsync(Guid sessionId, SenderType senderType);
        Task<IEnumerable<ChatMessageDto>> GetMessagesByMessageTypeAsync(Guid sessionId, MessageType messageType);
        Task<double> GetAverageResponseTimeAsync(Guid sessionId);
        Task<ChatMessageDto?> GetLastMessageInSessionAsync(Guid sessionId);
        Task DeleteMessageAsync(Guid id);
        Task UpdateMessageContentAsync(Guid id, string newContent);
    }
}