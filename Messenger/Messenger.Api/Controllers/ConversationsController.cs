using MediatR;
using Messenger.Api.AuthorizationAttributes;
using Messenger.Api.Extensions;
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
            Claim userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            ResultDto<IEnumerable<ConversationDto>> response = await _mediatoR
                .Send(new GetConversationsByUserIdQuery { UserId = new Guid(userIdClaim.Value) });

            return response.ToHttpResponse<IEnumerable<ConversationDto>>();
        }

        [HttpPost("privateConvarsationWithUser")]
        public async Task<IActionResult> CreatePrivateConversation([FromQuery] Guid userId)
        {
            Claim userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            ResultDto response = await _mediatoR.Send(new CreatePrivateConversationWithUserCommand
            {
                CreatorUserId = new Guid(userIdClaim.Value),
                UserId = userId,
            });

            return response.ToHttpResponse();
        }

        [HttpPost("groupConversation")]
        public async Task<IActionResult> CreateGroupConversation([FromBody] GroupModelDto groupModelDto)
        {
            Claim userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            ResultDto response = await _mediatoR.Send(new CreateGroupConversationCommand
            {
                CreatorUserId = new Guid(userIdClaim.Value),
                GroupModelDto = groupModelDto,
            });

            return response.ToHttpResponse(); 
        }


        [HttpDelete("{conversationId}")]
        [ParticipantInConversation]
        public async Task<IActionResult> DeleteConversation([FromRoute] Guid conversationId)
        {
            ResultDto response = await _mediatoR.Send(new DeleteConversationCommand
            {
                ConversationId = conversationId
            });

            return response.ToHttpResponse(); ;
        }
    }
}

