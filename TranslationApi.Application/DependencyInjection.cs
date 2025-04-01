
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TranslationApi.Application.Factories;
using TranslationApi.Application.Interfaces;
using TranslationApi.Application.Providers.Implementations;
using TranslationApi.Application.Services;

namespace TranslationApi.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register Services
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<IChatSessionService, ChatSessionService>();
            services.AddScoped<IChatMessageService, ChatMessageService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IAIModelService, AIModelService>();

            // Register Factory
            services.AddScoped<IAIProviderFactory, AIProviderFactory>();

            // Register Providers
            services.AddScoped<GeminiTranslationProvider>();

            // Add HttpClient for providers
            services.AddHttpClient<GeminiTranslationProvider>();

            return services;
        }
    }
}