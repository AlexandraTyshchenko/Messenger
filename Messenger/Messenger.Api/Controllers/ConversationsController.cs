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
[Route("api/[controller]")]
public class ConversationsController : BaseController
{
    private readonly IMediator _mediatoR;
    public ConversationsController(IMediator mediator)
    {
        _mediatoR = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetConversations()
    {
        ResultDto<IEnumerable<ConversationDto>> response = await _mediatoR
            .Send(new GetConversationsByUserIdQuery { UserId = UserId });

        return response.ToHttpResponse<IEnumerable<ConversationDto>>();
    }

    [HttpPost("private")]
    public async Task<IActionResult> CreatePrivateConversation([FromQuery] Guid userId)
    {
        ResultDto<ConversationWithParticipantsDto> response = await _mediatoR.Send(new CreatePrivateConversationWithUserCommand
        {
            CreatorUserId = UserId,
            UserId = userId,
        });

        return response.ToHttpResponse<ConversationWithParticipantsDto>();
    }

    [HttpPost("group")]
    public async Task<IActionResult> CreateGroupConversation([FromBody] GroupModelDto groupModelDto)
    {
        ResultDto<ConversationWithParticipantsDto> response = await _mediatoR.Send(new CreateGroupConversationCommand
        {
            CreatorUserId = UserId,
            Group = groupModelDto,
        });

        return response.ToHttpResponse<ConversationWithParticipantsDto>();
    }

    [HttpGet("{conversationId}")]
    [ConversationRoleFilter(Role.Participant)]
    public async Task<IActionResult> GetConversationById([FromRoute] Guid conversationId)
    {
        ResultDto<ConversationDto> response = await _mediatoR.Send(new GetConversationByIdQuery
        {
            ConversationId = conversationId
        });

        return response.ToHttpResponse<ConversationDto>();
    }

    [HttpDelete("{conversationId}")]
    [ConversationRoleFilter(Role.Admin)]
    public async Task<IActionResult> DeleteConversation([FromRoute] Guid conversationId)
    {
        ResultDto<AffectedRowsDto> response = await _mediatoR.Send(new DeleteConversationCommand
        {
            ConversationId = conversationId
        });

        return response.ToHttpResponse(); ;
    }
}

