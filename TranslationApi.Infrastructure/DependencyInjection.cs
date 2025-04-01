
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TranslationApi.Application.Factories;
using TranslationApi.Application.Interfaces;
using TranslationApi.Application.Providers.Implementations;
using TranslationApi.Application.Services;
using TranslationApi.Domain.Interfaces;
using TranslationApi.Infrastructure.Data;
using TranslationApi.Infrastructure.Repositories;

namespace TranslationApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<IAIModelRepository, AIModelRepository>();
            services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
            services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddScoped<IAIModelService, AIModelService>();
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<IChatSessionService, ChatSessionService>();
            services.AddScoped<IChatMessageService, ChatMessageService>();
            services.AddScoped<IFeedbackService, FeedbackService>();

            // AI Provider Factory
            services.AddScoped<IAIProviderFactory, AIProviderFactory>();

            // HTTP Client
            services.AddHttpClient<GeminiTranslationProvider>();

            return services;
        }
    }
}