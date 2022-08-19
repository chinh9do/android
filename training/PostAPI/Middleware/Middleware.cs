using BlogPost.Repository.Constain;
using System.Net;

namespace PostAPI.Middleware;

public class Middleware
{
    private readonly RequestDelegate _next;

    public Middleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (!context.Request.Headers.TryGetValue(AppSettingConstain.API_KEY, out var extractedApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Api Key was not provided.");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            var apiKey = appSettings.GetValue<string>(AppSettingConstain.API_KEY);

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
        catch
        {

        }
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Middleware>();
    }
}
