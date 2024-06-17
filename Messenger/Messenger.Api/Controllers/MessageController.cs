using MediatR;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Queries;
using Messenger.Infrastructure.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Messenger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Policy = "ParticipantInConversationPolicy")]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediatoR;

        public MessageController(IMediator mediator)
        {
            _mediatoR = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesByConversationId([FromQuery] Guid ConversationId)
        {
            var result = await _mediatoR.Send(new GetMessagesByConversationIdQuery { ConversationId = ConversationId });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddMessageToConversation([FromBody] MessageDto messageDto,
            [FromQuery] Guid conversationId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            var result = await _mediatoR.Send(new AddMessageToConversationCommand 
            { 
                MessageDto = messageDto, 
                ConversationId = conversationId, 
                SenderId = new Guid(userIdClaim.Value)
            });

            return Ok(result);
        }
    }
}
