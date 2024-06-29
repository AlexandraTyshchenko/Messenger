using MediatR;
using Messenger.Api.AuthorizationAttributes;
using Messenger.Api.Extensions;
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
            ResultDto<IEnumerable<MessageWithSenderDto>> response = await _mediatoR
                .Send(new GetMessagesByConversationIdQuery { ConversationId = conversationId });

            return response.ToHttpResponse<IEnumerable<MessageWithSenderDto>>();
        }

        [Route("api/Conversations/{conversationId}/[controller]")]
        [HttpPost]
        [ParticipantInConversation]
        public async Task<IActionResult> AddMessageToConversation([FromBody] MessageDto messageDto,
        [FromRoute] Guid conversationId)
        {
            Claim userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            ResultDto<MessageWithSenderDto> response = await _mediatoR.Send(new AddMessageToConversationCommand
            {
                MessageDto = messageDto,
                ConversationId = conversationId,
                SenderId = new Guid(userIdClaim.Value)
            });

            return response.ToHttpResponse<MessageWithSenderDto>();
        }

        [Route("api/[controller]/{messageId}/")]
        [HttpDelete]
        [MessageRemovalPermissions]
        public async Task<IActionResult> DeleteMessageFromConversation([FromRoute] Guid messageId)
        {
            ResultDto response = await _mediatoR.Send(new DeleteMessageCommand { MessageId = messageId });

            return response.ToHttpResponse();
        }
    }
}
