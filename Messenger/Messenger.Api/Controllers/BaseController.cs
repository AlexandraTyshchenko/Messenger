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
                if (_userId == null)
                {
                    throw new InvalidOperationException("User id is null.");
                }
                _userId = new Guid(userIdClaim.Value);
            }
            return _userId;
        }
    }
}
