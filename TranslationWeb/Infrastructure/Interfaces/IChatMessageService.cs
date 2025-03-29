using TranslationWeb.Models.ChatMessage;

namespace TranslationWeb.Infrastructure.Interfaces
{
    public interface IChatMessageService
    {
        Task<ChatMessageResponse> SendMessageAsync(SendMessageRequest request);
        Task<ChatMessageResponse> UpdateMessageAsync(UpdateMessageRequest request);
        Task<IEnumerable<ChatMessageResponse>> GetSessionMessagesAsync(Guid sessionId);
        Task<ChatMessageResponse> GetMessageByIdAsync(Guid messageId);
        Task<bool> DeleteMessageAsync(Guid messageId);
    }
}