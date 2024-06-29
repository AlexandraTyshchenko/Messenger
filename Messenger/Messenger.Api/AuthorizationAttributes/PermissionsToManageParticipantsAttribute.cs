using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Messenger.Api.AuthorizationAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermissionsToManageParticipantsAttribute : TypeFilterAttribute
    {
        public PermissionsToManageParticipantsAttribute()
            : base(typeof(PermissionsToManageParticipantsHandler))
        {
        }
    }

    public class PermissionsToManageParticipantsHandler : IAsyncAuthorizationFilter
    {
        private IParticipantRepository _participantRepository;

        public PermissionsToManageParticipantsHandler(IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            if (httpContext == null)
            {
                context.Result = new JsonResult(new { message = "Internal Server Error." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return;
            }

            if (!httpContext.Request.RouteValues.TryGetValue("userToDeleteId", out var userIdObj) ||
                !Guid.TryParse(userIdObj?.ToString(), out var userToDeleteId))
            {
                context.Result = new JsonResult(new { message = "User Id is null or invalid." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            if (!httpContext.Request.RouteValues.TryGetValue("conversationId", out var conversationIdObj) ||
              !Guid.TryParse(conversationIdObj?.ToString(), out var conversationId))
            {
                context.Result = new JsonResult(new { message = "Conversation Id is null or invalid." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            Guid currentUserId = new Guid(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            ParticipantInConversation participantInConversation = await _participantRepository
                .GetParticipantFromConversationAsync(currentUserId, conversationId);

            if (participantInConversation == null)
            {
                context.Result = new JsonResult(new
                {
                    participantInConversation = $"User wasn`t found in conversation."
                })
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
                return;
            }

            if (!(userToDeleteId == currentUserId || participantInConversation.Role == Role.Admin)) //check if user deletes themself or is admin
            {
                context.Result = new JsonResult(new
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
}