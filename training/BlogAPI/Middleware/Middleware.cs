using BlogPost.Repository.Models;
using BlogPost.Service.Helpers;
using System.Net;

namespace BlogAPI.Middleware;

public class Middleware
{
    private readonly RequestDelegate _next;

    private readonly ILoggerManager _logger;

    public Middleware(RequestDelegate next, ILoggerManager logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UserNotFoundException unfEx)
        {
            _logger.LogError($"Something went wrong: {unfEx}");
            //context.Response
            await HandleExceptionAsync(context, unfEx);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var message = exception switch
        {
            UserNotFoundException => exception.Message,
            _ => "Internal Server Error."
        };

        await context.Response.WriteAsync(new ErrorDetailsModel()
        {
            StatusCode = context.Response.StatusCode,
            Message = message,
        }.ToString());
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Middleware>();
    }
}
