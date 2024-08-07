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
        if (response.Success)
        {
            return new OkObjectResult(response.Payload);
        }
        else
        {
            var result = new
            {
                ErrorMessage = response.ErrorMessage,
                Payload = response.Payload
            };
            return StatusCode((int)response.HttpStatusCode, result);
        }
    }
    private static ObjectResult StatusCode([ActionResultStatusCode] int statusCode, [ActionResultObjectValue] object? value)
    {
        return new ObjectResult(value)
        {
            StatusCode = statusCode
        };
    }
}
