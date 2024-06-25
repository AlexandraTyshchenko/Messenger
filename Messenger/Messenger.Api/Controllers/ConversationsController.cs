using MediatR;
using Messenger.Api.AuthorizationAttributes;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Queries;
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
            var response = await _mediatoR.Send(new GetConversationByUserIdQuery { UserId = new Guid(userIdClaim.Value) });

            return Ok(response.Payload);
        }

        [HttpPost("privateConvarsationWithUser")]
        public async Task<IActionResult> CreatePrivateConversation([FromQuery] Guid userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var response = await _mediatoR.Send(new CreatePrivateConversationWithUserCommand
            {
                CreatorUserId = new Guid(userIdClaim.Value),
                UserId = userId,
            });

            return response.Success ? Ok() : StatusCode((int)response.HttpStatusCode, response.ErrorMessage);
        }

        [HttpPost("groupConversation")]
        public async Task<IActionResult> CreateGroupConversation([FromBody] GroupModelDto groupModelDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            await _mediatoR.Send(new CreateGroupConversationCommand
            {
                CreatorUserId = new Guid(userIdClaim.Value),
                GroupModelDto = groupModelDto,
            });

            return Ok();
        }

        [ParticipantInConversation]

        [HttpDelete("{conversationId}")]
        public async Task<IActionResult> DeletePrivateConversation([FromRoute] Guid conversationId)
        {
            var response = await _mediatoR.Send(new DeleteConversationCommand
            {
                ConversationId = conversationId
            });

            return response.Success ? Ok() : StatusCode((int)response.HttpStatusCode, response.ErrorMessage);
        }
    }
}

