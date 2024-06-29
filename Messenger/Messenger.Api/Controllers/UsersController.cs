using MediatR;
using Messenger.Api.Extensions;
using Messenger.Business.Dtos;
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
            ResultDto<IEnumerable<UserBasicInfoDto>> response = await _mediatoR.Send(new GetUsersByUserNameQuery
            {
                UserName = userName,
            });

            return response.ToHttpResponse<IEnumerable<UserBasicInfoDto>>();
        }
    }
}
