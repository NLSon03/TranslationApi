﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using System.Text;
using TranslationApi.API.Configurations;
using TranslationApi.Application.Interfaces;
using TranslationApi.Application.Services;
using TranslationApi.Domain.Entities;
using TranslationApi.Infrastructure;
using TranslationApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.WebHost.UseUrls("http://localhost:5292");

// Services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Cấu hình các yêu cầu mật khẩu
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;

    // Cấu hình đăng nhập
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Cấu hình khóa tài khoản
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddSignInManager<SignInManager<ApplicationUser>>();

//Đăng ký cấu hình JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
// 3. Cấu hình JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
#pragma warning disable CS8602 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
#pragma warning disable CS8602 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        ClockSkew = TimeSpan.Zero
    };
});


// Other services
builder.Services.AddAutoMapper(typeof(TranslationApi.Application.Mappings.AIModelMappingProfile).Assembly);
// Cấu hình CORS
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddCustomRateLimiting(builder.Configuration);
// Add health checks
//builder.Services.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Cấu hình Gemini Service
builder.Services.AddHttpClient<GeminiTranslationService>(client =>
{
    var apiUrl = builder.Configuration["GeminiApi:ApiUrl"];
    if (string.IsNullOrEmpty(apiUrl))
    {
        throw new ArgumentNullException(nameof(apiUrl), "API URL cannot be null or empty.");
    }

    client.BaseAddress = new Uri(apiUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("x-goog-api-key", builder.Configuration["GeminiApi:ApiKey"]);
});

builder.Services.AddScoped<ITranslationService, GeminiTranslationService>();

// Cấu hình Swagger với hỗ trợ JWT
builder.Services.AddSwaggerGenConfigured();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Translation API v1");
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.EnableTryItOutByDefault();
    });

    // Thêm thông tin về URL
    app.Logger.LogInformation("Swagger URL: {url}", "http://localhost:5292/swagger");
}

app.UseCustomSecurityHeaders();
app.UseRequestLogging();
app.ConfigureExceptionHandler();

app.UseHttpsRedirection();
app.UseRateLimiter();

// Áp dụng CORS middleware
app.UseCors("MyAllowSpecificOrigins");

// Đặt UseAuthentication trước UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Log startup message
var urls = app.Urls.Select(url => $"- {url}").ToList();
app.Logger.LogInformation("Application started. Available at URLs:");
foreach (var url in urls)
{
    app.Logger.LogInformation(url);
}
app.Logger.LogInformation("Swagger is available at: http://localhost:5292/swagger");

app.Run();