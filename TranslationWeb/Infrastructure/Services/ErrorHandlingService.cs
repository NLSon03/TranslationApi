
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TranslationWeb.Infrastructure.Interfaces;

namespace TranslationWeb.Infrastructure.Services;

public class ErrorHandlingService : IErrorHandlingService
{
    private readonly GlobalExceptionHandler _globalExceptionHandler;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;

    public ErrorHandlingService(
        GlobalExceptionHandler globalExceptionHandler,
        NavigationManager navigationManager,
        IJSRuntime jsRuntime)
    {
        _globalExceptionHandler = globalExceptionHandler;
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }

    public async Task HandleErrorAsync(Exception exception)
    {
        await _globalExceptionHandler.HandleErrorAsync(exception);
    }

    public async Task HandleHttpErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            await _jsRuntime.InvokeVoidAsync("console.error", "HTTP Error:", response.StatusCode, errorContent);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Unauthorized:
                    _navigationManager.NavigateTo("/auth/login");
                    break;

                case System.Net.HttpStatusCode.Forbidden:
                    await _globalExceptionHandler.HandleErrorAsync(
                        new UnauthorizedAccessException("Bạn không có quyền truy cập tài nguyên này."));
                    break;

                case System.Net.HttpStatusCode.NotFound:
                    if (!_navigationManager.Uri.EndsWith("/error", StringComparison.OrdinalIgnoreCase))
                    {
                        _navigationManager.NavigateTo("/error");
                    }
                    break;

                default:
                    await _globalExceptionHandler.HandleErrorAsync(
                        new HttpRequestException($"HTTP Error: {response.StatusCode} - {errorContent}"));
                    break;
            }
        }
        catch (Exception ex)
        {
            await _globalExceptionHandler.HandleErrorAsync(ex);
        }
    }

    public async Task HandleApiErrorAsync(string errorMessage)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("console.error", "API Error:", errorMessage);
            await _globalExceptionHandler.HandleErrorAsync(new Exception(errorMessage));
        }
        catch (Exception ex)
        {
            // Fallback error handling if JSRuntime fails
            _navigationManager.NavigateTo("/error");
            Console.Error.WriteLine($"Error handling API error: {ex.Message}");
        }
    }
}