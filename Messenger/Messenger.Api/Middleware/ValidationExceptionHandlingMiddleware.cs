using FluentValidation;
using System.Net;

namespace Messenger.Api.Middleware;

public class ValidationExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionHandlingMiddleware> _logger;

    public ValidationExceptionHandlingMiddleware(RequestDelegate next, ILogger<ValidationExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var errors = new
        {
            Errors = exception.Errors
        };

        await context.Response.WriteAsJsonAsync(errors);
        _logger.LogError(exception, exception.Message);
    }
}
