using Messenger.Infrastructure;
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
    private readonly IUnitOfWork _unitOfWork;

    public PermissionsToManageMessagesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        Message message = await _unitOfWork.Messages.GetMessageByIdAsync(messageId);

        if (message == null)
        {
            context.Result = new ObjectResult(new { message = $"Conversation with id {messageId} wasn`t found." })
            {
                StatusCode = StatusCodes.Status404NotFound
            };
            return;
        }

        ParticipantInConversation participantInConversation = await _unitOfWork.Participants
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