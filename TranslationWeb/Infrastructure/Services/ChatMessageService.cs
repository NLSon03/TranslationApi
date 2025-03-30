using System.Net.Http.Json;
using System.Text.Json;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Models.ChatMessage;

namespace TranslationWeb.Infrastructure.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/ChatMessage";
        private readonly JsonSerializerOptions _jsonOptions;

        public ChatMessageService(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClient;
            _jsonOptions = jsonOptions;
        }

        public async Task<ChatMessageResponse> SendMessageAsync(SendMessageRequest request)
        {
            try
            {
                // Kiểm tra và log request
                if (request == null)
                {
                    Console.WriteLine("Lỗi: request là null");
                    return new ChatMessageResponse();
                }

                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    Console.WriteLine("Lỗi: Content là null hoặc rỗng");
                    return new ChatMessageResponse();
                }

                // Log chi tiết request
                Console.WriteLine("Sending request with data:");
                Console.WriteLine($"- SessionId: {request.SessionId}");
                Console.WriteLine($"- Content: {request.Content}");
                Console.WriteLine($"- MessageType: {request.MessageType}");
                Console.WriteLine($"- FromLanguage: {request.FromLanguage}");
                Console.WriteLine($"- ToLanguage: {request.ToLanguage}");

                // Log request JSON
                var requestJson = JsonSerializer.Serialize(request, _jsonOptions);
                Console.WriteLine($"Request JSON: {requestJson}");

                // Gửi request trực tiếp, không cần wrapper
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, request, _jsonOptions);

                // Log response
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ChatMessageResponse>(_jsonOptions);
                    Console.WriteLine($"Deserialized response successfully: {result?.Id}");
                    return result ?? new ChatMessageResponse();
                }

                return new ChatMessageResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SendMessageAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Lỗi khi cập nhật tin nhắn: {errorContent}");
                return new ChatMessageResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception khi cập nhật tin nhắn: {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Exception khi lấy tin nhắn của phiên: {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Exception khi lấy tin nhắn theo ID: {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Exception khi xóa tin nhắn: {ex.Message}");
                return false;
            }
        }
    }
}