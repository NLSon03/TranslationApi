using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TranslationWeb.Infrastructure.Services.LocalStorage;
using TranslationWeb.Models.Auth;

namespace TranslationWeb.Infrastructure.Services.Http
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> _logger;
        private readonly LocalStorageService _localStorage;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public HttpService(HttpClient httpClient, ILogger<HttpService> logger, LocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _logger = logger;
            _localStorage = localStorage;
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                await AddJwtHeader();
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing GET request to {Url}", url);
                throw;
            }
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            try
            {
                await AddJwtHeader();
                var content = new StringContent(
                    JsonSerializer.Serialize(data), 
                    Encoding.UTF8, 
                    "application/json"
                );
                
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing POST request to {Url}", url);
                throw;
            }
        }

        public async Task<T?> PutAsync<T>(string url, object data)
        {
            try
            {
                await AddJwtHeader();
                var content = new StringContent(
                    JsonSerializer.Serialize(data), 
                    Encoding.UTF8, 
                    "application/json"
                );
                
                var response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing PUT request to {Url}", url);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string url)
        {
            try
            {
                await AddJwtHeader();
                var response = await _httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing DELETE request to {Url}", url);
                throw;
            }
        }

        private async Task AddJwtHeader()
        {
            try
            {
                var authResult = await _localStorage.GetItemAsync<AuthResponse>("user_session");
                if (authResult != null && !string.IsNullOrEmpty(authResult.Token))
                {
                    _logger.LogInformation("Đang thêm JWT Token vào header: {Token}", authResult.Token.Substring(0, Math.Min(10, authResult.Token.Length)) + "...");
                    
                    // Xóa header authorization cũ nếu có
                    if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    }
                    
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.Token);
                }
                else
                {
                    _logger.LogWarning("Không tìm thấy token hoặc token rỗng");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm JWT token vào request");
            }
        }
    }
} 