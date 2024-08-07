using MediatR;
using Messenger.Api.Extensions;
using Messenger.Business.Dtos;
using Messenger.Business.Queries;
using Messenger.Infrastructure.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly IMediator _mediatoR;

    public UsersController(IMediator mediator)
    {
        _mediatoR = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> SearchUsersByUserName([FromQuery] SearchUsersParams searchUserParams,
        [FromQuery] PaginationParams paginationParams)
    {
        ResultDto<IPagedEntities<UserBasicInfoDto>> response = await _mediatoR
            .Send(new GetUsersByUserNameQuery
            {
                UserName = searchUserParams.UserName,
                PaginationParams = paginationParams
            });

        return response.ToHttpResponse();
    }
}
