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
    public class ParticipantsController : ControllerBase
    {
        private readonly IMediator _mediatoR;
        public ParticipantsController(IMediator mediator)
        {
            _mediatoR = mediator;
        }

        [Route("api/Conversations/{conversationId}/Participants")]
        [HttpGet]
        [ParticipantInConversation(isGroup: true)]
        public async Task<IActionResult> GetParticipantsByConversationId([FromRoute] Guid conversationId)
        {
            ResultDto<IEnumerable<ParticipantDto>> response = await _mediatoR.Send(new GetParticipantsByConversationIdQuery
            {
                ConversationId = conversationId
            });

            return response.ToHttpResponse<IEnumerable<ParticipantDto>>();
        }

        [Route("api/Conversations/{conversationId}/Participants/AddParticipantToGroupConversation")]
        [HttpPost]
      //  [ParticipantInConversation(isGroup: true)]
        public async Task<IActionResult> AddParticipantsToConversation([FromBody] Guid[] userIds, [FromRoute] Guid conversationId)
        {
            ResultDto response = await _mediatoR.Send(new AddParticipantToConversationCommand
            {
                ConversationId = conversationId,
                UserIds = userIds
            });

            return response.ToHttpResponse();
        }

        [Route("api/Participants/{participantInConversationId}")]
        [HttpDelete]
        [ParticipantRemovalPermissions]
        public async Task<IActionResult> DeleteParticipantsFromConversation([FromRoute] Guid participantInConversationId)
        {
            ResultDto response = await _mediatoR.Send(new DeleteParticipantFromConversationCommand
            {
                ParticipantInConversationId = participantInConversationId
            });

            return response.ToHttpResponse();
        }
    }
}
