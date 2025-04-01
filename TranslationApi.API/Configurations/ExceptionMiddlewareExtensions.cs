using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace TranslationApi.API.Configurations
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var exception = error.Error;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            error = "Internal Server Error",
                            message = exception.Message
                        }.ToString()));
                    }
                });
            });
        }
    }
}
