using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Polly;
using System.Text.Json.Serialization;
using TranslationWeb;
using TranslationWeb.Core.Authentication;
using TranslationWeb.Core.Services;
using TranslationWeb.Infrastructure.Interfaces;
using TranslationWeb.Infrastructure.Services;
using TranslationWeb.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Disable Browser Link in development
if (builder.HostEnvironment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}

// Configure JSON options
builder.Services.AddScoped(sp =>
{
    var options = new System.Text.Json.JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    return options;
});

// Configure HTTP client with retry policy and JSON options
builder.Services.AddHttpClient("API", (sp, client) =>
{
    client.BaseAddress = new Uri("http://localhost:5292");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddTransientHttpErrorPolicy(policy => policy
    .WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))) // Exponential backoff
.AddTransientHttpErrorPolicy(policy => policy
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30))); // Circuit breaker

// Register HttpClient factory with JSON options
builder.Services.AddScoped(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("API");
    return client;
});

// Add error handling services
builder.Services.AddScoped<GlobalExceptionHandler>();
builder.Services.AddScoped<IErrorHandlingService, ErrorHandlingService>();

// Register core services
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<HttpService>();

// Register Error handling
builder.Services.AddScoped<ErrorViewModel>();

// Register Authentication and Authorization
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Register AuthenticationStateProvider
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

// Register other services
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ITranslationApiService, TranslationApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<IChatSessionService, ChatSessionService>();
builder.Services.AddScoped<IAIModelService, AIModelService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

// Register Blazored Toast
builder.Services.AddBlazoredToast();

// Build and configure the application
var host = builder.Build();

// Configure global error handling
var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
{
    if (args.ExceptionObject is Exception ex)
    {
        var errorHandler = host.Services.GetRequiredService<GlobalExceptionHandler>();
        errorHandler.HandleError(ex);
    }
};

await host.RunAsync();