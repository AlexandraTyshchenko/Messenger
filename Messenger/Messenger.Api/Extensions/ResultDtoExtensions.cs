using Messenger.Business.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Messenger.Api.Extensions;

public static class ResultDtoExtensions
{
    public static IActionResult ToHttpResponse(this ResultDto response)
    {
        return response.Success ? new OkResult() : StatusCode((int)response.HttpStatusCode, response.ErrorMessage);
    }

    public static IActionResult ToHttpResponse<T>(this ResultDto<T> response)
    {
        return response.Success ? new OkObjectResult(response.Payload) : StatusCode((int)response.HttpStatusCode, response.ErrorMessage);
    }

    private static ObjectResult StatusCode([ActionResultStatusCode] int statusCode, [ActionResultObjectValue] object? value)
    {
        return new ObjectResult(value)
        {
            StatusCode = statusCode
        };
    }
}
