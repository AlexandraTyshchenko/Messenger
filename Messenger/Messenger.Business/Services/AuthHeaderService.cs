using Messenger.Business.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Messenger.Business.Services;

public class AuthHeaderService : IAuthHeaderService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthHeaderService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetAuthToken()
    {
        string authToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        return authToken;
    }
}
