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
    [Route("api/Conversations/{conversationId}/[controller]")]
    [ParticipantInConversation]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesByConversationId([FromRoute] Guid conversationId)
        {
            ResultDto<IEnumerable<MessageWithSenderDto>> response = await _mediator
                .Send(new GetMessagesByConversationIdQuery { ConversationId = conversationId });

            return response.ToHttpResponse<IEnumerable<MessageWithSenderDto>>();
        }

        [HttpPost]
        public async Task<IActionResult> AddMessageToConversation([FromBody] MessageDto messageDto, [FromRoute] Guid conversationId)
        {
            Claim userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            ResultDto<MessageWithSenderDto> response = await _mediator.Send(new AddMessageToConversationCommand
            {
                MessageDto = messageDto,
                ConversationId = conversationId,
                SenderId = new Guid(userIdClaim.Value)
            });

            return response.ToHttpResponse<MessageWithSenderDto>();
        }

        [HttpDelete("{messageId}")]
        [PermissionsToManageMessages]
        public async Task<IActionResult> DeleteMessageFromConversation([FromRoute] Guid messageId)
        {
            ResultDto response = await _mediator.Send(new DeleteMessageCommand { MessageId = messageId });

            return response.ToHttpResponse();
        }
    }
}
