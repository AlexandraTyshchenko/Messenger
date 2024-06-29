using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Messenger.Api.AuthorizationAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ParticipantInConversationAttribute : TypeFilterAttribute
    {
        public ParticipantInConversationAttribute(bool isGroup = false, bool isAdmin = false)
            : base(typeof(ParticipantInConversationFilter))
        {
            Arguments = new object[] { isGroup, isAdmin };
        }
    }

    public class ParticipantInConversationFilter : IAsyncAuthorizationFilter
    {
        private readonly bool _isGroup;
        private readonly bool _isAdmin;

        private IParticipantRepository _participantRepository;
        private IConversationRepository _conversationRepository;

        public ParticipantInConversationFilter(bool isGroup, bool isAdmin, IParticipantRepository participantRepository)
        {
            _isGroup = isGroup;
            _isAdmin = isAdmin;
            _participantRepository = participantRepository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            _participantRepository = httpContext.RequestServices.GetRequiredService<IParticipantRepository>();
            _conversationRepository = httpContext.RequestServices.GetRequiredService<IConversationRepository>();

            if (httpContext == null)
            {
                context.Result = new JsonResult(new { message = "Internal Server Error" })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
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

            Guid userId = new Guid(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            ParticipantInConversation participantInConversation = await _participantRepository
                .GetParticipantFromConversationAsync(userId, conversationId);

            if (participantInConversation == null)
            {
                context.Result = new JsonResult(new
                {
                    message = $"Participant with user id {userId}" +
                    $" wasn`t found in conversation with id {conversationId}."
                })
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
                return;
            }

            if (_isGroup && participantInConversation.Conversation.Group == null)
            {
                context.Result = new JsonResult(new { message = $"Conversation with id {conversationId} is not a group conversation." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            if (_isAdmin && participantInConversation.Role != Role.Admin)
            {
                context.Result = new JsonResult(new
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
}
