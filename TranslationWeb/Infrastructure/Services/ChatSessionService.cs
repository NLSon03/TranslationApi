using System.Net.Http.Json;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.ChatSession;

namespace TranslationWeb.Infrastructure.Services
{
    public class ChatSessionService : IChatSessionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/ChatSession";

        public ChatSessionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ChatSessionResponse> CreateSessionAsync(CreateSessionRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ChatSessionResponse>()
                        ?? new ChatSessionResponse();
                }

                // Handle error response
                return new ChatSessionResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new ChatSessionResponse();
            }
        }

        public async Task<ChatSessionResponse> GetSessionByIdAsync(Guid sessionId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ChatSessionResponse>($"{_baseUrl}/{sessionId}")
                    ?? new ChatSessionResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new ChatSessionResponse();
            }
        }

        public async Task<IEnumerable<ChatSessionResponse>> GetUserSessionsAsync(string userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<ChatSessionResponse>>($"{_baseUrl}/user/{userId}")
                    ?? new List<ChatSessionResponse>();
            }
            catch (Exception)
            {
                // Log exception
                return new List<ChatSessionResponse>();
            }
        }

        public async Task<ChatSessionResponse> EndSessionAsync(Guid sessionId)
        {
            try
            {
                var request = new EndSessionRequest { SessionId = sessionId };
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{sessionId}/end", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ChatSessionResponse>()
                        ?? new ChatSessionResponse();
                }

                // Handle error response
                return new ChatSessionResponse();
            }
            catch (Exception)
            {
                // Log exception
                return new ChatSessionResponse();
            }
        }

        public async Task<bool> DeleteSessionAsync(Guid sessionId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{sessionId}");
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