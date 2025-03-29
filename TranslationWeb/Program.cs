using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TranslationWeb;
using TranslationWeb.Core.Authentication;
using TranslationWeb.Infrastructure.Services.Api;
using TranslationWeb.Infrastructure.Services.Api.Auth;
using TranslationWeb.Infrastructure.Services.Api.Feedback;
using TranslationWeb.Infrastructure.Services.Http;
using TranslationWeb.Infrastructure.Services.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HTTP client
builder.Services.AddScoped(sp => {
    var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5292") };
    // Cấu hình header mặc định
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    return httpClient;
});

// Register Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Register services
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<ITranslationApiService, TranslationApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();

await builder.Build().RunAsync();
