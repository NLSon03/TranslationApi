using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TranslationApi.Application.Interfaces;
using TranslationApi.Application.Services;
using TranslationApi.Domain.Interfaces;
using TranslationApi.Infrastructure.Data;
using TranslationApi.Infrastructure.Repositories;

namespace TranslationApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)),
                ServiceLifetime.Scoped);

            // Đăng ký các Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAIModelRepository, AIModelRepository>();
            services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
            services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Đăng ký các Service
            services.AddScoped<IAIModelService, AIModelService>();
            services.AddScoped<IChatSessionService, ChatSessionService>();
            services.AddScoped<IChatMessageService, ChatMessageService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}