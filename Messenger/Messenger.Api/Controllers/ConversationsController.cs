using MediatR;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Queries;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Messenger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ConversationsController : ControllerBase
    {
        private readonly IMediator _mediatoR;
        public ConversationsController(IMediator mediator)
        {
            _mediatoR = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetConversations()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var result = await _mediatoR.Send(new GetConversationByUserIdQuery { UserId = new Guid(userIdClaim.Value) });
            return Ok(result);
        }

        [HttpPost("privateConvarsationWithUser")]
        public async Task<IActionResult> CreatePrivateConversation([FromQuery] Guid userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var result = await _mediatoR.Send(new CreatePrivateConversationWithUserCommand
            {
                CreatorUserId = new Guid(userIdClaim.Value),
                UserId = userId,
            });
            return Ok(result);
        }

        [HttpPost("groupConversation")]
        public async Task<IActionResult> CreateGroupConversation([FromBody] GroupModelDto groupModelDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            var result = await _mediatoR.Send(new CreateGroupConversationCommand
            {
                CreatorUserId = new Guid(userIdClaim.Value),
                GroupModelDto = groupModelDto,
            });

            return Ok(result);
        }
    }
}

