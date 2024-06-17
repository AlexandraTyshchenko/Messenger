using MediatR;
using Messenger.Business.Commands;
using Messenger.Business.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "ParticipantInConversationPolicy")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IMediator _mediatoR;
        public ParticipantsController(IMediator mediator)
        {
            _mediatoR = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetParticipantsByConversationId([FromQuery] Guid conversationId)
        {
            var result = await _mediatoR.Send(new GetParticipantsByConversationIdQuery { ConversationId = conversationId });

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminInConversationPolicy")]
        public async Task<IActionResult> AddParticipantsToConversation([FromBody] Guid[] userIds, [FromQuery] Guid conversationId)
        {
            var result = await _mediatoR.Send(new AddParticipantToConversationCommand
            {
                ConversationId = conversationId,
                UserIds = userIds
            });

            return Ok(result);
        }
    }
}
