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
    public class MessageRemovalPermissionsAttribute : TypeFilterAttribute
    {
        public MessageRemovalPermissionsAttribute()
          : base(typeof(MessageRemovalPermissionsHandler))
        {
        }
    }

    public class MessageRemovalPermissionsHandler : IAsyncAuthorizationFilter
    {
        private IMessageRepository _messageRepository;
        private IParticipantRepository _participantRepository;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            _participantRepository = httpContext.RequestServices.GetRequiredService<IParticipantRepository>();
            _messageRepository = httpContext.RequestServices.GetRequiredService<IMessageRepository>();

            if (httpContext == null)
            {
                context.Result = new JsonResult(new { message = "Internal Server Error" })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return;
            }

            if (!httpContext.Request.RouteValues.TryGetValue("messageId", out var messageIdObj) ||
                !Guid.TryParse(messageIdObj?.ToString(), out var messageId))
            {
                context.Result = new JsonResult(new { message = "Message ID is null or invalid." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            try
            {
                Message message = await _messageRepository.GetMessageByIdAsync(messageId);

                var userId = new Guid(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                ParticipantInConversation currentUser = await _participantRepository
                    .GetParticipantFromConversationAsync(userId, message.Conversation.Id);

                if (currentUser == null)
                {
                    context.Result = new JsonResult(new
                    {
                        message = "User doesn't have permissions to delete message," +
                        " as  user not a part of conversation where message was sent"
                    })
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                    return;
                }

                if (!(message.Sender.Id == userId || currentUser.Role == Role.Admin))
                {
                    context.Result = new JsonResult(new { message = "User doesn't have permissions to delete message" })
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
