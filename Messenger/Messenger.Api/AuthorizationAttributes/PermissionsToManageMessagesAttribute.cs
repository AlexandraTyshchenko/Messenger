using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Messenger.Api.AuthorizationAttributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PermissionsToManageMessagesAttribute : TypeFilterAttribute
{
    public PermissionsToManageMessagesAttribute()
      : base(typeof(PermissionsToManageMessagesHandler))
    {
    }
}

public class PermissionsToManageMessagesHandler : IAsyncAuthorizationFilter
{
    private readonly IMessageRepository _messageRepository;
    private readonly IParticipantRepository _participantRepository;

    public PermissionsToManageMessagesHandler(IMessageRepository messageRepository,
        IParticipantRepository participantRepository)
    {
        _messageRepository = messageRepository;
        _participantRepository = participantRepository;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;

        if (httpContext == null)
        {
            context.Result = new ObjectResult(new { message = "Internal Server Error." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            return;
        }

        if (!httpContext.Request.RouteValues.TryGetValue("messageId", out var messageIdObj) ||
            !Guid.TryParse(messageIdObj?.ToString(), out var messageId))
        {
            context.Result = new ObjectResult(new { message = "Message ID is null or invalid." })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        if (!httpContext.Request.RouteValues.TryGetValue("conversationId", out var conversationIdObj) ||
          !Guid.TryParse(conversationIdObj?.ToString(), out var conversationId))
        {
            context.Result = new ObjectResult(new { message = "Conversation ID is null or invalid." })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        Guid userId = new Guid(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        Message message = await _messageRepository.GetMessageByIdAsync(messageId);

        if (message == null)
        {
            context.Result = new ObjectResult(new { message = $"Conversation with id {messageId} wasn`t found." })
            {
                StatusCode = StatusCodes.Status404NotFound
            };
            return;
        }

        ParticipantInConversation participantInConversation = await _participantRepository
            .GetParticipantFromConversationAsync(userId, conversationId);

        if (participantInConversation == null)
        {
            context.Result = new ObjectResult(new
            {
                participantInConversation = $"User wasn`t found in conversation."
            })
            {
                StatusCode = StatusCodes.Status404NotFound
            };
            return;
        }

        if (!(message.Sender.Id == userId || participantInConversation.Role == Role.Admin))
        {
            context.Result = new ObjectResult(new
            {
                participantInConversation = $"User doesn`t have permissions to delete the message."
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }
    }
}