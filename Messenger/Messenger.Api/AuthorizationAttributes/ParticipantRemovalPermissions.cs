using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Exceptions;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Messenger.Api.AuthorizationAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ParticipantRemovalPermissionsAttribute : TypeFilterAttribute
    {
        public ParticipantRemovalPermissionsAttribute()
            : base(typeof(ParticipantRemovalPermissionsHandler))
        {
        }
    }

    public class ParticipantRemovalPermissionsHandler : IAsyncAuthorizationFilter
    {
        private IParticipantRepository _participantRepository;

        public ParticipantRemovalPermissionsHandler(IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            if (httpContext == null)
            {
                context.Result = new JsonResult(new { message = "Internal Server Error" })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return;
            }

            if (!httpContext.Request.RouteValues.TryGetValue("participantInConversationId", out var participantInConversationIdObj) ||
                !Guid.TryParse(participantInConversationIdObj?.ToString(), out var participantInConversationId))
            {
                context.Result = new JsonResult(new { message = "Participant ID is null or invalid." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            try
            {
                ParticipantInConversation participantInConversation = await _participantRepository.GetParticipantByIdAsync(participantInConversationId);

                var userId = new Guid(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                ParticipantInConversation currentUser = await _participantRepository.GetParticipantFromConversationAsync(userId, participantInConversation.Conversation.Id);

                if (currentUser == null)
                {
                    context.Result = new JsonResult(new
                    {
                        message = "User doesn't have permissions to delete participant," +
                        " as current user and user to remove are not in the same conversation"
                    })
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                    return;
                }
                if (!(participantInConversation.Id == currentUser.Id || currentUser.Role == Role.Admin))
                {
                    context.Result = new JsonResult(new { message = "User doesn't have permissions to delete user from conversation." })
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    return;
                }
            }
            catch (NotFoundException ex)
            {
                context.Result = new JsonResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status404NotFound,
                };
            }
            catch (CustomException ex)
            {
                context.Result = new JsonResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                };
            }
        }
    }
}
