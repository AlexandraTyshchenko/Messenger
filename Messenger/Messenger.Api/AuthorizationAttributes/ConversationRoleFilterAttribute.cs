using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Messenger.Api.AuthorizationAttributes
{
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

        private IParticipantRepository _participantRepository;
        private IConversationRepository _conversationRepository;

        public ConversationRoleFilterHandler(Role role, IParticipantRepository participantRepository)
        {
            _role = role;
            _participantRepository = participantRepository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            _participantRepository = httpContext.RequestServices.GetRequiredService<IParticipantRepository>();
            _conversationRepository = httpContext.RequestServices.GetRequiredService<IConversationRepository>();

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

            ParticipantInConversation participantInConversation = await _participantRepository
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

            if (_role == Role.Participant && participantInConversation.User.Id != userId)
            {
                context.Result = new ObjectResult(new
                {
                    message = "Current user does not have " +
                    "privileges in this conversation."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
        }
    }
}
