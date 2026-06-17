using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using JobTracker.Api.Exceptions;

namespace JobTracker.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Request.ContentType = "application/json";

        int statusCode;
        string error;

        Console.WriteLine(exception.StackTrace);

        switch (exception)
        {
            case ValidationException:
                statusCode = StatusCodes.Status400BadRequest;
                error = "BadRequest";
                break;

            case InvalidCredentialsException or UnauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                error = "Unauthorized";
                break;

            case JobApplicationNotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                error = "NotFound";
                break;
            
            case UsernameAlreadyExistsException:
                statusCode = StatusCodes.Status409Conflict;
                error = "Conflict";
                break;
            
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                error = "InternalServerError";
                break;
        }

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            Error = error,
            Message = exception.Message
        }));
    }
}