﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
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
//builder.WebHost.UseUrls("http://localhost:5292");

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

// Cấu hình Cookie cho xác thực
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

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
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
    options.SaveTokens = true;
});

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetIsOriginAllowed(origin => true); // Cho phép tất cả nguồn gốc
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

builder.Services.AddScoped<ITranslationService, TranslationService>();

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

// Configure Kestrel endpoints
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5292, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    serverOptions.ListenAnyIP(5293, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

// Đảm bảo WebRootPath tồn tại
var webRootPath = builder.Environment.WebRootPath;
if (string.IsNullOrEmpty(webRootPath))
{
    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    Directory.CreateDirectory(webRootPath);
    builder.Environment.WebRootPath = webRootPath;
}

// Tạo thư mục uploads nếu chưa tồn tại
var uploadPath = Path.Combine(webRootPath, "uploads", "avatars");
Directory.CreateDirectory(uploadPath);

if (!Directory.Exists(uploadPath))
{
    throw new Exception($"Không thể tạo thư mục upload: {uploadPath}");
}

var app = builder.Build();

// Luôn bật HTTPS Redirection dù là môi trường nào
app.UseHttpsRedirection();

// Bật Static Files Middleware
app.UseStaticFiles();

// Middleware để chặn hoàn toàn HTTP request trong Development
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Scheme == "http")
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("HTTP is not supported. Please use HTTPS.");
            return;
        }
        await next();
    });
}

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