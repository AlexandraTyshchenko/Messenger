using MediatR;
using Messenger.Api.AuthorizationAttributes;
using Messenger.Api.Extensions;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Queries;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/Conversations/{conversationId}/[controller]")]
[ConversationRoleFilter(Role.Participant)]
public class MessagesController : BaseController
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMessagesByConversationId([FromRoute] Guid conversationId,
       [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator
            .Send(new GetMessagesByConversationIdQuery
            {
                ConversationId = conversationId,
                PaginationParams = paginationParams
            });

        return response.ToHttpResponse();
    }

    [HttpPost]
    public async Task<IActionResult> AddMessageToConversation([FromForm] MessageDto message, [FromRoute] Guid conversationId)
    {
        var response = await _mediator.Send(new AddMessageToConversationCommand
        {
            Message = message,
            ConversationId = conversationId,
            SenderId = UserId,
        });

        return response.ToHttpResponse();
    }

    [HttpPost("spam")]
    public async Task<IActionResult> StartSpam(
        [FromRoute] Guid conversationId,
        [FromBody] SpamRequestDto request)
    {
        await _mediator.Send(new StartSpamCommand
        {
            ConversationId = conversationId,
            Text = request.Text,
            Lambda = request.Lambda,
            DurationSeconds = request.DurationSeconds,
            SenderId = UserId
        });

        return Ok(new { message = "Spam finished successfully" });
    }

    [HttpPost("spam-real")]
    public async Task<IActionResult> StartRealSpam(
    [FromRoute] Guid conversationId,
    [FromBody] SpamRequestDto request)
    {
        await _mediator.Send(new StartRealSpamCommand
        {
            ConversationId = conversationId,
            Text = request.Text,
            Lambda = request.Lambda,
            DurationSeconds = request.DurationSeconds,
            SenderId = UserId
        });

        return Ok(new { message = "Real spam finished successfully" });
    }

    [HttpDelete("{messageId}")]
    [PermissionsToManageMessages]
    public async Task<IActionResult> DeleteMessageFromConversation([FromRoute] Guid messageId)
    {
        var response = await _mediator.Send(new DeleteMessageCommand { MessageId = messageId });

        return response.ToHttpResponse();
    }
}
