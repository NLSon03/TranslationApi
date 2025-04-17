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
                // Nếu đã kiểm tra trong 30 giây gần đây, trả về kết quả trước đó
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
                        // Kiểm tra kết nối server khi thất bại lần cuối cùng
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


        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));
                    await AddJwtHeader();

                    var requestContent = new StringContent(
                        JsonSerializer.Serialize(data, _jsonOptions),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await _httpClient.PostAsync(url, requestContent, cts.Token);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorized();
                        continue;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API response: Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, responseContent);

                    // Kiểm tra mã trạng thái trước khi phân tích phản hồi
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("API trả về lỗi: {StatusCode}, {Content}",
                            response.StatusCode, responseContent);

                        // Nếu là API đăng nhập hoặc đăng ký, cố gắng đọc thông báo lỗi từ server
                        if (url.Contains("/login") || url.Contains("/register"))
                        {
                            try
                            {
                                var errorResponse = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
                                if (errorResponse != null)
                                {
                                    return errorResponse; // Trả về đối tượng lỗi để client có thể hiển thị
                                }
                            }
                            catch { /* Bỏ qua lỗi phân tích JSON */ }
                        }

                        throw new HttpRequestException($"API error: {response.StatusCode}", null, response.StatusCode);
                    }

                    try
                    {
                        var result = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
                        if (result == null)
                        {
                            throw new JsonException("Deserialized result is null");
                        }
                        return result;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize response");
                        throw new HttpRequestException($"Failed to parse API response: {responseContent}", ex);
                    }
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

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));
                    await AddJwtHeader();

                    var requestContent = new StringContent(
                        JsonSerializer.Serialize(data, _jsonOptions),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await _httpClient.PutAsync(url, requestContent, cts.Token);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorized();
                        continue;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API response: Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, responseContent);

                    try
                    {
                        var result = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
                        if (result == null)
                        {
                            throw new JsonException("Deserialized result is null");
                        }
                        return result;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize response");
                        throw new HttpRequestException($"Failed to parse API response: {responseContent}", ex);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Request timeout for PUT {Url}", url);
                    if (i == _maxRetries - 1) throw;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error executing PUT request to {Url}", url);
                    if (i == _maxRetries - 1) throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error executing PUT request to {Url}", url);
                    throw;
                }

                await Task.Delay((int)Math.Min(100 * Math.Pow(2, i), 1000));
            }

            throw new HttpRequestException($"Failed to execute PUT request to {url} after {_maxRetries} attempts");
        }
        public async Task<TResponse?> PostMultipartAsync<TResponse>(string url, MultipartFormDataContent content)
        {
            if (!await CheckServerConnection())
                throw new HttpRequestException("Server is not available", null, System.Net.HttpStatusCode.ServiceUnavailable);

            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));
                    await AddJwtHeader();

                    var response = await _httpClient.PostAsync(url, content, cts.Token);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorized();
                        continue;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException($"API error: {response.StatusCode}", null, response.StatusCode);

                    return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
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
                await Task.Delay((int)Math.Min(100 * Math.Pow(2, i), 1000));
            }

            throw new HttpRequestException($"Failed to execute POST request to {url} after {_maxRetries} attempts");
        }
        public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest data)
        {
            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));
                    await AddJwtHeader();

                    var content = new StringContent(
                        JsonSerializer.Serialize(data, _jsonOptions),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await _httpClient.PatchAsync(url, content, cts.Token);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorized();
                        continue;
                    }

                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Request timeout for PATCH {Url}", url);
                    if (i == _maxRetries - 1) throw;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error executing PATCH request to {Url}", url);
                    if (i == _maxRetries - 1) throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error executing PATCH request to {Url}", url);
                    throw;
                }

                await Task.Delay((int)Math.Min(100 * Math.Pow(2, i), 1000));
            }

            throw new HttpRequestException($"Failed to execute PATCH request to {url} after {_maxRetries} attempts");
        }

        public async Task<T?> DeleteAsync<T>(string url)
        {
            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));
                    await AddJwtHeader();

                    var response = await _httpClient.DeleteAsync(url, cts.Token);

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
                    _logger.LogWarning("Request timeout for DELETE {Url}", url);
                    if (i == _maxRetries - 1) throw;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error executing DELETE request to {Url}", url);
                    if (i == _maxRetries - 1) throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error executing DELETE request to {Url}", url);
                    throw;
                }

                await Task.Delay((int)Math.Min(100 * Math.Pow(2, i), 1000));
            }

            throw new HttpRequestException($"Failed to execute DELETE request to {url} after {_maxRetries} attempts");
        }
        public async Task<bool> DeleteAsync(string url)
        {
            try
            {
                await DeleteAsync<object>(url);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task AddJwtHeader()
        {
            try
            {
                // Lấy token trực tiếp từ localStorage thay vì phụ thuộc vào user_session
                var token = await _localStorage.GetItemAsync<string>("authToken");
                var expirationStr = await _localStorage.GetItemAsync<string>("authExpiration");

                var tokenExpired = false;
                if (!string.IsNullOrEmpty(expirationStr) && DateTime.TryParse(expirationStr, out var expiration))
                {
                    tokenExpired = expiration <= DateTime.Now;
                }

                // Kiểm tra token có hiệu lực
                if (!string.IsNullOrEmpty(token) && !tokenExpired)
                {
                    _logger.LogInformation("Đang thêm JWT Token vào header: {Token}",
                        token.Substring(0, Math.Min(10, token.Length)) + "...");

                    // Xóa header authorization cũ nếu có
                    if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    }

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    // Thử lấy từ user_session nếu cách trên thất bại (để tương thích ngược)
                    var authResult = await _localStorage.GetItemAsync<AuthResponse>("user_session");
                    if (authResult != null && !string.IsNullOrEmpty(authResult.Token) && !authResult.IsAccessTokenExpired())
                    {
                        _logger.LogInformation("Sử dụng token từ user_session: {Token}",
                            authResult.Token.Substring(0, Math.Min(10, authResult.Token.Length)) + "...");

                        // Lưu token để sử dụng trong tương lai
                        await _localStorage.SetItemAsync("authToken", authResult.Token);
                        await _localStorage.SetItemAsync("authExpiration", authResult.ExpiresAt.ToString("o"));

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
                        _logger.LogWarning("Không tìm thấy token hợp lệ");
                        // Chỉ xử lý unauthorized nếu không phải là endpoint xác thực
                        if (!_navigationManager.Uri.Contains("/auth/login", StringComparison.OrdinalIgnoreCase) &&
                            !_navigationManager.Uri.Contains("/auth/register", StringComparison.OrdinalIgnoreCase) &&
                            !_navigationManager.Uri.Contains("/auth/google-callback", StringComparison.OrdinalIgnoreCase))
                        {
                            await HandleUnauthorized();
                        }
                    }
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
                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("authExpiration");

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
                if (!_navigationManager.Uri.Contains("/auth/login", StringComparison.OrdinalIgnoreCase) &&
                    !_navigationManager.Uri.Contains("/auth/google-callback", StringComparison.OrdinalIgnoreCase))
                {
                    _navigationManager.NavigateTo($"/auth/login?returnUrl={returnUrl}", forceLoad: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý unauthorized");
            }
        }

        public string GetBaseUrl()
        {
            return _navigationManager.BaseUri;
        }

        public Task NavigateToExternalUrl(string url)
        {
            _navigationManager.NavigateTo(url, true);
            return Task.CompletedTask;
        }
    }
}