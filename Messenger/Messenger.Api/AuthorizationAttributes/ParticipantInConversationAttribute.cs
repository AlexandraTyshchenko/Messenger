using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Exceptions;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Messenger.Api.AuthorizationAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ParticipantInConversationAttribute : TypeFilterAttribute
    {
        public ParticipantInConversationAttribute(bool isGroup = false)
            : base(typeof(ParticipantInConversationFilter))
        {
            Arguments = new object[] { isGroup };
        }
    }

    public class ParticipantInConversationFilter : IAsyncAuthorizationFilter
    {
        private readonly bool _isGroup;

        private IParticipantRepository _participantRepository;
        private IConversationRepository _conversationRepository;

        public ParticipantInConversationFilter(bool isGroup)
        {
            _isGroup = isGroup;
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
                context.Result = new JsonResult(new { message = "Conversation ID is null or invalid." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            if (!await IsValidConversation(httpContext, conversationId, context))
            {
                return;
            }

            string userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                ParticipantInConversation participant =
                    await _participantRepository.GetParticipantFromConversationAsync(new Guid(userId), conversationId);

                if (!IsValidParticipant(httpContext, participant, conversationId, context))
                {
                    return;
                }
            }
            catch (CustomException ex)
            {
                context.Result = new JsonResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
        }

        private async Task<bool> IsValidConversation(HttpContext httpContext, Guid conversationId, AuthorizationFilterContext context)
        {
            try
            {
                Conversation conversation = await _conversationRepository.GetConversationByIdAsync(conversationId);

                if (_isGroup && conversation.Group == null)
                {
                    context.Result = new JsonResult(new { message = $"Conversation with id {conversation.Id} is not a group conversation." })
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    return false;
                }
                return true;
            }
            catch (CustomException ex)
            {
                context.Result = new JsonResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return false;
            }
        }

        private bool IsValidParticipant(HttpContext httpContext, ParticipantInConversation participant,
            Guid conversationId, AuthorizationFilterContext context)
        {
            if (participant == null)
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                context.Result = new JsonResult(new { message = $"No participant found for user ID {userId} in conversation ID {conversationId}." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return false;
            }
            return true;
        }
    }
}
