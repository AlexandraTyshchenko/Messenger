using MediatR;
using Messenger.Business.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediatoR;

        public UsersController(IMediator mediator)
        {
            _mediatoR = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsersByUserName([FromQuery] string userName)
        {
            var response = await _mediatoR.Send(new GetUsersByUserNameQuery
            {
                UserName = userName,
            });

            return Ok(response.Payload);
        }
    }
}
