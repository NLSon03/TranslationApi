using System.Net.Http.Json;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.ChatMessage;

namespace TranslationWeb.Infrastructure.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/chatmessages";

        public ChatMessageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ChatMessageResponse> SendMessageAsync(SendMessageRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ChatMessageResponse>()
                        ?? new ChatMessageResponse();
                }

                // Handle error response
                return new ChatMessageResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new ChatMessageResponse();
            }
        }

        public async Task<ChatMessageResponse> UpdateMessageAsync(UpdateMessageRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{request.MessageId}", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ChatMessageResponse>()
                        ?? new ChatMessageResponse();
                }

                // Handle error response
                return new ChatMessageResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new ChatMessageResponse();
            }
        }

        public async Task<IEnumerable<ChatMessageResponse>> GetSessionMessagesAsync(Guid sessionId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<ChatMessageResponse>>($"{_baseUrl}/session/{sessionId}")
                    ?? new List<ChatMessageResponse>();
            }
            catch (Exception)
            {
                // Log exception
                return new List<ChatMessageResponse>();
            }
        }

        public async Task<ChatMessageResponse> GetMessageByIdAsync(Guid messageId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ChatMessageResponse>($"{_baseUrl}/{messageId}")
                    ?? new ChatMessageResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new ChatMessageResponse();
            }
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{messageId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }
    }
}