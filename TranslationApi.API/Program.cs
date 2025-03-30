using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using System.Text;
using TranslationApi.Application.Interfaces;
using TranslationApi.Application.Services;
using TranslationApi.Domain.Entities;
using TranslationApi.Infrastructure;
using TranslationApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Đặt URL cụ thể
builder.WebHost.UseUrls("http://localhost:5292");

// Đăng ký Infrastructure và Application services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Đăng ký Identity với đầy đủ tính năng
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

// Cấu hình JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAutoMapper(typeof(TranslationApi.Application.Mappings.AIModelMappingProfile).Assembly);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
        ClockSkew = TimeSpan.Zero // Bỏ khoảng thời gian gia hạn mặc định 5 phút
    };
});

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Add health checks
builder.Services.AddHealthChecks();

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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Translation API",
        Description = "API for translating text using Gemini API",
        Contact = new OpenApiContact { Name = "Translation App Team" },
        License = new OpenApiLicense { Name = "Use under License" }
    });

    // Thêm cấu hình JWT cho Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

app.UseCors(MyAllowSpecificOrigins);

// Đặt UseAuthentication trước UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/api/health").RequireCors(MyAllowSpecificOrigins);

// Log startup message
var urls = app.Urls.Select(url => $"- {url}").ToList();
app.Logger.LogInformation("Application started. Available at URLs:");
foreach (var url in urls)
{
    app.Logger.LogInformation(url);
}
app.Logger.LogInformation("Swagger is available at: http://localhost:5292/swagger");

app.Run();