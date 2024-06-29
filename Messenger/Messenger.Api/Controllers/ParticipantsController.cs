using MediatR;
using Messenger.Api.AuthorizationAttributes;
using Messenger.Api.Extensions;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/Conversations/{conversationId}/[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ParticipantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ParticipantInConversation]
        public async Task<IActionResult> GetParticipantsByConversationId([FromRoute] Guid conversationId)
        {
            ResultDto<IEnumerable<UserBasicInfoDto>> response = await _mediator.Send(new GetParticipantsByConversationIdQuery
            {
                ConversationId = conversationId
            });

            return response.ToHttpResponse<IEnumerable<UserBasicInfoDto>>();
        }

        [HttpPost("AddParticipantToGroupConversation")]
        [ParticipantInConversation(isGroup: true)]
        public async Task<IActionResult> AddParticipantsToConversation([FromBody] Guid[] userIds, [FromRoute] Guid conversationId)
        {
            ResultDto response = await _mediator.Send(new AddParticipantToConversationCommand
            {
                ConversationId = conversationId,
                UserIds = userIds
            });

            return response.ToHttpResponse();
        }

        [HttpDelete("DeleteUserFromConversation/{userToDeleteId}")]
        [ParticipantInConversation(isGroup: true)]
        [PermissionsToManageParticipants]
        public async Task<IActionResult> DeleteParticipantsFromConversation([FromRoute] Guid userToDeleteId,
            [FromRoute] Guid conversationId)
        {
            ResultDto response = await _mediator.Send(new DeleteParticipantFromConversationCommand
            {
                UserToDeleteId = userToDeleteId,
                ConversationId = conversationId
            });

            return response.ToHttpResponse();
        }
    }
}
