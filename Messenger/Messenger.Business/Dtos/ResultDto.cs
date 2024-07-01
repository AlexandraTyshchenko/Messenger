using System.Net;

namespace Messenger.Business.Dtos;

public class ResultDto<T> : ResultDto
{
    public T Payload { get; set; }
}

public class ResultDto
{
    public bool Success { get; set; }
    public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;
    public string ErrorMessage { get; set; } = string.Empty;

    public static ResultDto SuccessResult(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        if ((int)statusCode / 100 != 2)
        {
            throw new InvalidOperationException("Status code must be successful (2xx).");
        }

        var result = new ResultDto
        {
            Success = true,
            HttpStatusCode = statusCode
        };

        return result;
    }

    public static ResultDto<TReturnObject> SuccessResult<TReturnObject>(TReturnObject payload, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        if ((int)statusCode / 100 != 2)
        {
            throw new InvalidOperationException("Status code must be successful (2xx).");
        }

        var result = new ResultDto<TReturnObject>
        {
            Success = true,
            Payload = payload,
            HttpStatusCode = statusCode
        };

        return result;
    }

    public static ResultDto FailureResult(
        HttpStatusCode statusCode,
        string StatusMessage)
    {
        if ((int)statusCode / 100 < 4)
        {
            throw new InvalidOperationException("Status code must be client or server error code (4xx or 5xx).");
        }


        var result = new ResultDto
        {
            Success = false,
            HttpStatusCode = statusCode,
            ErrorMessage = StatusMessage
        };

        return result;
    }

    public static ResultDto<TReturnObject> FailureResult<TReturnObject>(HttpStatusCode statusCode,
        string statusMessage, TReturnObject payload = null)
        where TReturnObject : class
    {
        if ((int)statusCode / 100 < 4)
        {
            throw new InvalidOperationException("Status code must be client or server error code (4xx or 5xx).");
        }

        var result = new ResultDto<TReturnObject>
        {
            Success = false,
            HttpStatusCode = statusCode,
            Payload = payload,
            ErrorMessage = statusMessage
        };

        return result;
    }
}
