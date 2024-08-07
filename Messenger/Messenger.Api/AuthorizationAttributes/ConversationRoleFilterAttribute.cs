using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Messenger.Api.AuthorizationAttributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ConversationRoleFilterAttribute : TypeFilterAttribute
{
    public ConversationRoleFilterAttribute(Role role)
        : base(typeof(ConversationRoleFilterHandler))
    {
        Arguments = new object[] { role };
    }
}

public class ConversationRoleFilterHandler : IAsyncAuthorizationFilter
{
    private readonly Role _role;

    private readonly IUnitOfWork _unitOfWork;

    public ConversationRoleFilterHandler(Role role, IUnitOfWork unitOfWork)
    {
        _role = role;
        _unitOfWork = unitOfWork;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;

        if (httpContext == null)
        {
            context.Result = new ObjectResult(new { message = "Internal Server Error" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            return;
        }

        if (!httpContext.Request.RouteValues.TryGetValue("conversationId", out var conversationIdObj) ||
            !Guid.TryParse(conversationIdObj?.ToString(), out var conversationId))
        {
            context.Result = new ObjectResult(new { message = "Conversation Id is null or invalid." })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        Guid userId = new Guid(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        ParticipantInConversation participantInConversation = await _unitOfWork.Participants
            .GetParticipantFromConversationAsync(userId, conversationId);

        if (participantInConversation == null)
        {
            context.Result = new ObjectResult(new
            {
                message = $"Participant with user id {userId}" +
                $" wasn`t found in conversation with id {conversationId}."
            })
            {
                StatusCode = StatusCodes.Status404NotFound
            };
            return;
        }

        if (_role == Role.Admin && participantInConversation.Role != Role.Admin)
        {
            context.Result = new ObjectResult(new
            {
                message = "Current user does not have " +
                "administrative privileges in this conversation."
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }
    }
}
