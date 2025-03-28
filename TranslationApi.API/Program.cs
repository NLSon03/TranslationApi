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

// Đăng ký Infrastructure và Application services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Đăng ký Identity
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };
});
// Cấu hình services
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Translation API",
        Version = "v1",
        Description = "API for translating text using Gemini API",
        Contact = new OpenApiContact { Name = "Translation App Team" }
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();