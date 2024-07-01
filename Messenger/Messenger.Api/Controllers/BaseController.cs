using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Messenger.Api.Controllers;

public class BaseController : ControllerBase
{
    private Guid _userId;

    protected Guid UserId
    {
        get
        {
            if (_userId == Guid.Empty)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    _userId = new Guid(userIdClaim.Value);
                }
            }
            return _userId;
        }
    }
}
