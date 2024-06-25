using MediatR;
using Messenger.Api.AuthorizationAttributes;
using Messenger.Business.Commands;
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
        [ParticipantInConversation( isGroup: true)]
        [HttpGet]
        public async Task<IActionResult> GetParticipantsByConversationId([FromRoute] Guid conversationId)
        {
            var response = await _mediatoR.Send(new GetParticipantsByConversationIdQuery { ConversationId = conversationId });

            return Ok(response.Payload);
        }

        [Route("api/Conversations/{conversationId}/Participants/AddParticipantToGroupConversation")]
        [ParticipantInConversation( isGroup: true)]
        [HttpPost]
        public async Task<IActionResult> AddParticipantsToConversation([FromBody] Guid[] userIds, [FromRoute] Guid conversationId)
        {
            var response = await _mediatoR.Send(new AddParticipantToConversationCommand
            {
                ConversationId = conversationId,
                UserIds = userIds
            });

            return response.Success ? Created() : StatusCode((int)response.HttpStatusCode, response.ErrorMessage);
        }

        [Route("api/Participants/{participantInConversationId}")]
        [ParticipantRemovalPermissions]
        [HttpDelete]
        public async Task<IActionResult> DeleteParticipantsFromConversation([FromRoute] Guid participantInConversationId)
        {
            
            var response = await _mediatoR.Send(new DeleteParticipantFromConversationCommand
            {
                ParticipantInConversationId = participantInConversationId
            });

            return response.Success ? Ok() : StatusCode((int)response.HttpStatusCode, response.ErrorMessage);
        }
    }
}
