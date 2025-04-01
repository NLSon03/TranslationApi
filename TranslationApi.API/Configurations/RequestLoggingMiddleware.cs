namespace TranslationApi.API.Configurations
{
    public static class RequestLoggingMiddleware
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                await next();
                stopwatch.Stop();
                var logMessage = $"[{context.Request.Method}] {context.Request.Path} - {context.Response.StatusCode} - {stopwatch.ElapsedMilliseconds}ms";
            });
            return app;
        }
    }
}
