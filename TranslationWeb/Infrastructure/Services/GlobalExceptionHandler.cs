
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TranslationWeb.Infrastructure.Services;

public class GlobalExceptionHandler
{
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;

    public GlobalExceptionHandler(NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }

    public async Task HandleErrorAsync(Exception exception)
    {
        // Log error
        Console.Error.WriteLine($"Error: {exception.Message}");
        Console.Error.WriteLine($"StackTrace: {exception.StackTrace}");

        try
        {
            // Show error in console
            await _jsRuntime.InvokeVoidAsync("console.error", "An error occurred:", exception.Message);

            // Navigate to error page if not already there
            if (!_navigationManager.Uri.EndsWith("/error", StringComparison.OrdinalIgnoreCase))
            {
                _navigationManager.NavigateTo("/error");
            }
        }
        catch
        {
            // If JavaScript interop fails, just navigate to error page
            _navigationManager.NavigateTo("/error");
        }
    }

    public void HandleError(Exception exception)
    {
        // Synchronous version for cases where async isn't possible
        Console.Error.WriteLine($"Error: {exception.Message}");
        Console.Error.WriteLine($"StackTrace: {exception.StackTrace}");

        if (!_navigationManager.Uri.EndsWith("/error", StringComparison.OrdinalIgnoreCase))
        {
            _navigationManager.NavigateTo("/error");
        }
    }
}