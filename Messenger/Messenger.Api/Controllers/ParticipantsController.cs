using MediatR;
using Messenger.Api.AuthorizationAttributes;
using Messenger.Api.Extensions;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Queries;
using Messenger.Infrastructure.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/conversations/{conversationId}/[controller]")]
public class ParticipantsController : BaseController
{
    private readonly IMediator _mediator;

    public ParticipantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ConversationRoleFilter(Role.Participant)]
    public async Task<IActionResult> GetParticipantsByConversationId([FromRoute] Guid conversationId)
    {
        ResultDto<IEnumerable<UserBasicInfoDto>> response = await _mediator.Send(new GetParticipantsByConversationIdQuery
        {
            ConversationId = conversationId
        });

        return response.ToHttpResponse<IEnumerable<UserBasicInfoDto>>();
    }

    [HttpPost]
    [ConversationRoleFilter(Role.Participant)]
    public async Task<IActionResult> AddParticipantsToConversation([FromBody] Guid[] userIds, [FromRoute] Guid conversationId)
    {
        ResultDto<AffectedRowsDto> response = await _mediator.Send(new AddParticipantToConversationCommand
        {
            ConversationId = conversationId,
            UserIds = userIds
        });

        return response.ToHttpResponse();
    }

    [HttpDelete("{userId}")]
    [ConversationRoleFilter(Role.Admin)]
    public async Task<IActionResult> DeleteParticipantsFromConversation([FromRoute] Guid userId,
        [FromRoute] Guid conversationId)
    {
        ResultDto<AffectedRowsDto> response = await _mediator.Send(new DeleteParticipantFromConversationCommand
        {
            UserId = userId,
            ConversationId = conversationId
        });

        return response.ToHttpResponse();
    }

    [HttpDelete]
    [ConversationRoleFilter(Role.Participant)]
    public async Task<IActionResult> LeaveConversation([FromRoute] Guid conversationId)
    {
        ResultDto response = await _mediator.Send(new DeleteParticipantFromConversationCommand
        {
            UserId = UserId,
            ConversationId = conversationId
        });

        return response.ToHttpResponse();
    }
}
