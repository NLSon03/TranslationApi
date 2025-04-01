namespace TranslationApi.API.Configurations
{
    public static class SecurityHeadersMiddleware
    {
        public static IApplicationBuilder UseCustomSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
                await next();
            });
            return app;
        }
    }
}
