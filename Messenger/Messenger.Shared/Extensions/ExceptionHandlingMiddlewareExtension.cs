using Messenger.Shared.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Messenger.Shared.Extensions;

public static class ExceptionHandlingMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
