using System.Net.Http.Json;
using System.Text.Json;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.ChatSession;

namespace TranslationWeb.Infrastructure.Services
{
    public class ChatSessionService : IChatSessionService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl = "api/ChatSession";

        public ChatSessionService(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClient;
            _jsonOptions = jsonOptions;
        }

        public async Task<ChatSessionResponse> CreateSessionAsync(CreateSessionRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}?modelId={request.AIModelId}", new { });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ChatSessionResponse>();
                    if (result != null)
                    {
                        return result;
                    }
                }

                Console.WriteLine($"Failed to create session. Status code: {response.StatusCode}");
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error content: {error}");

                return new ChatSessionResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating session: {ex.Message}");
                return new ChatSessionResponse();
            }
        }

        public async Task<ChatSessionResponse> GetSessionByIdAsync(Guid sessionId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ChatSessionResponse>($"{_baseUrl}/{sessionId}/messages")
                    ?? new ChatSessionResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting session with messages: {ex.Message}");
                return new ChatSessionResponse();
            }
        }

        public async Task<IEnumerable<ChatSessionResponse>> GetUserSessionsAsync(string userId)
        {
            try
            {
                Console.WriteLine("Calling API to get user sessions...");
                var response = await _httpClient.GetAsync($"{_baseUrl}/my");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Raw API Response: {jsonString}");

                    try
                    {
                        var sessions = JsonSerializer.Deserialize<IEnumerable<ChatSessionResponse>>(
                            jsonString,
                            _jsonOptions
                        );

                        if (sessions != null)
                        {
                            var sessionsList = sessions.ToList();
                            Console.WriteLine($"Successfully deserialized {sessionsList.Count} sessions");

                            foreach (var session in sessionsList)
                            {
                                Console.WriteLine(session.ToString());
                            }

                            return sessionsList;
                        }
                        else
                        {
                            Console.WriteLine("Deserialized sessions is null");
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON Deserialization error: {ex.Message}");
                        Console.WriteLine($"JSON content that failed to deserialize: {jsonString}");
                    }
                }
                else
                {
                    Console.WriteLine($"API returned error status code: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response content: {errorContent}");
                }

                return new List<ChatSessionResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user sessions: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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