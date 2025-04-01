
using TranslationApi.Application.DTOs;

namespace TranslationApi.Application.Providers
{
    public interface IChatProvider : IModelProvider
    {
        Task<ChatMessageDto> GenerateResponseAsync(ChatMessageDto request, string sessionId);
        Task<bool> ValidatePrompt(string prompt);
        Task<int> GetMaxTokens();
    }
}