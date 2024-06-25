using Azure;
using MediatR;
using Messenger.Api.AuthorizationAttributes;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Queries;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Messenger.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediatoR;

        public MessagesController(IMediator mediator)
        {
            _mediatoR = mediator;
        }

        [Route("api/Conversations/{conversationId}/[controller]")]
        [ParticipantInConversation]
        [HttpGet]
        public async Task<IActionResult> GetMessagesByConversationId([FromRoute] Guid conversationId)
        {
            var response = await _mediatoR.Send(new GetMessagesByConversationIdQuery { ConversationId = conversationId });

            return response.Success ? Ok(response.Payload) : StatusCode((int)response.HttpStatusCode, response.ErrorMessage); 
        }

        [Route("api/Conversations/{conversationId}/[controller]")]
        [ParticipantInConversation]
        [HttpPost]
        public async Task<IActionResult> AddMessageToConversation([FromBody] MessageDto messageDto,
        [FromRoute] Guid conversationId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            var response = await _mediatoR.Send(new AddMessageToConversationCommand
            {
                MessageDto = messageDto,
                ConversationId = conversationId,
                SenderId = new Guid(userIdClaim.Value)
            });

            return response.Success ? Ok(response.Payload) : StatusCode((int)response.HttpStatusCode, response.ErrorMessage);
        }

        [Route("api/[controller]/{messageId}/")]
        [MessageRemovalPermissions]
        [HttpDelete]
        public async Task<IActionResult> DeleteMessageToConversation( [FromRoute] Guid messageId)
        {
            var response = await _mediatoR.Send(new DeleteMessageCommand { MessageId = messageId });

            return response.Success ? Ok() : StatusCode((int)response.HttpStatusCode, response.ErrorMessage); ;
        }
    }
}
