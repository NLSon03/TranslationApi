using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TranslationWeb.Models.Auth;

namespace TranslationWeb.Infrastructure.Services
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> _logger;
        private readonly LocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public event Action? OnAuthenticationFailed;

        public HttpService(
            HttpClient httpClient,
            ILogger<HttpService> logger,
            LocalStorageService localStorage,
            NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _logger = logger;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        private readonly int _maxRetries = 3;
        private readonly int _timeoutSeconds = 30;
        private readonly SemaphoreSlim _connectionCheckLock = new SemaphoreSlim(1, 1);
        private DateTime _lastConnectionCheck = DateTime.MinValue;
        private bool _isServerAvailable = true;

        private async Task<bool> CheckServerConnection()
        {
            try
            {
                // 如果在最近30秒内已经检查过，直接返回上次的结果
                if (DateTime.Now - _lastConnectionCheck < TimeSpan.FromSeconds(30))
                {
                    return _isServerAvailable;
                }

                await _connectionCheckLock.WaitAsync();
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    var response = await _httpClient.GetAsync("api/health", cts.Token);
                    _isServerAvailable = response.IsSuccessStatusCode;
                    _lastConnectionCheck = DateTime.Now;
                    return _isServerAvailable;
                }
                finally
                {
                    _connectionCheckLock.Release();
                }
            }
            catch
            {
                _isServerAvailable = false;
                _lastConnectionCheck = DateTime.Now;
                return false;
            }
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            if (!await CheckServerConnection())
            {
                throw new HttpRequestException("Server is not available", null, System.Net.HttpStatusCode.ServiceUnavailable);
            }

            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));
                    await AddJwtHeader();
                    
                    var response = await _httpClient.GetAsync(url, cts.Token);
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorized();
                        continue;
                    }
                    
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Request timeout for GET {Url}", url);
                    if (i == _maxRetries - 1) throw;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error executing GET request to {Url}", url);
                    if (i == _maxRetries - 1)
                    {
                        // 如果是最后一次重试，检查服务器连接
                        if (!await CheckServerConnection())
                        {
                            throw new HttpRequestException("Server is not available", ex, System.Net.HttpStatusCode.ServiceUnavailable);
                        }
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error executing GET request to {Url}", url);
                    throw;
                }
                
                // Wait before retry with exponential backoff
                if (i < _maxRetries - 1)
                {
                    var delay = (int)Math.Min(100 * Math.Pow(2, i), 1000);
                    await Task.Delay(delay);
                }
            }
            
            throw new HttpRequestException($"Failed to execute GET request to {url} after {_maxRetries} attempts");
        }

        //private async Task HandleUnauthorized()
        //{
        //    await _localStorage.RemoveItemAsync("user_session");
        //    _httpClient.DefaultRequestHeaders.Authorization = null;
        //}

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));
                    await AddJwtHeader();

                    var content = new StringContent(
                        JsonSerializer.Serialize(data),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await _httpClient.PostAsync(url, content, cts.Token);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorized();
                        continue;
                    }

                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Request timeout for POST {Url}", url);
                    if (i == _maxRetries - 1) throw;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error executing POST request to {Url}", url);
                    if (i == _maxRetries - 1) throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error executing POST request to {Url}", url);
                    throw;
                }

                // Wait before retry with exponential backoff
                await Task.Delay((int)Math.Min(100 * Math.Pow(2, i), 1000));
            }

            throw new HttpRequestException($"Failed to execute POST request to {url} after {_maxRetries} attempts");
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

        public async Task<T?> PatchAsync<T>(string url, object data)
        {
            try
            {
                await AddJwtHeader();
                var content = new StringContent(
                    JsonSerializer.Serialize(data ?? new { }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PatchAsync(url, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing PATCH request to {Url}", url);
                throw;
            }
        }

        public async Task PatchAsync(string url, object data)
        {
            try
            {
                await AddJwtHeader();
                var content = new StringContent(
                    JsonSerializer.Serialize(data ?? new { }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PatchAsync(url, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing PATCH request to {Url}", url);
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
                
                // Kiểm tra token có hiệu lực
                if (authResult != null && !string.IsNullOrEmpty(authResult.Token))
                {
                    if (authResult.ExpiresAt <= DateTime.Now)
                    {
                        _logger.LogWarning("Token đã hết hạn");
                        await HandleUnauthorized();
                        return;
                    }

                    _logger.LogInformation("Đang thêm JWT Token vào header: {Token}", 
                        authResult.Token.Substring(0, Math.Min(10, authResult.Token.Length)) + "...");

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
                    await HandleUnauthorized();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm JWT token vào request");
                await HandleUnauthorized();
            }
        }

        private async Task HandleUnauthorized()
        {
            try
            {
                // Xóa token khỏi local storage
                await _localStorage.RemoveItemAsync("user_session");
                
                // Xóa Authorization header
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }

                // Thông báo cho các component khác về việc xác thực thất bại
                OnAuthenticationFailed?.Invoke();

                // Lưu URL hiện tại để redirect sau khi đăng nhập lại
                var returnUrl = Uri.EscapeDataString(_navigationManager.Uri);
                
                // Kiểm tra để tránh redirect loop
                if (!_navigationManager.Uri.Contains("/auth/login", StringComparison.OrdinalIgnoreCase))
                {
                    _navigationManager.NavigateTo($"/auth/login?returnUrl={returnUrl}", forceLoad: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý unauthorized");
            }
        }
    }
}