using System.Threading.RateLimiting;

namespace TranslationApi.API.Configurations
{
    public static class RateLimitingConfiguration
    {
        public static void AddCustomRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            var rateLimitSettings = new RateLimitSettings();

            configuration.GetSection("RateLimitSettings").Bind(rateLimitSettings);
            services.AddRateLimiter(options =>
            {
                options.AddPolicy("api", httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitSettings.MaxRequest,
                            Window = TimeSpan.FromSeconds(rateLimitSettings.WindowInSeconds),
                            SegmentsPerWindow = rateLimitSettings.SegmentsPerWindow,
                        }
                        ));
            });
        }

        public class RateLimitSettings
        {
            public int MaxRequest { get; set; }
            public int WindowInSeconds { get; set; }
            public int SegmentsPerWindow { get; set; }

        }
    }
}
