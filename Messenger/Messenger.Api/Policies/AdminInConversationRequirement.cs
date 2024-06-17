using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Api.Policies
{
    public class AdminInConversationRequirement : IAuthorizationRequirement
    {
        public string ConversationId { get; }

        public AdminInConversationRequirement(string conversationId = "")
        {
            ConversationId = conversationId;
        }

        public class AdminInConversationRequirementHandler : AuthorizationHandler<AdminInConversationRequirement>
        {
            private readonly UserManager<User> _userManager;
            private readonly IParticipantRepository _participantRepository;

            public AdminInConversationRequirementHandler(UserManager<User> userManager, IParticipantRepository participantRepository)
            {
                _userManager = userManager;
                _participantRepository = participantRepository;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminInConversationRequirement requirement)
            {
                var httpContext = context.Resource as HttpContext;
                if (httpContext == null)
                {
                    context.Fail();
                    return;
                }

                var conversationId = httpContext.Request.Query["conversationId"].ToString();
                ;
                if (string.IsNullOrEmpty(conversationId))
                {
                    context.Fail();
                    return;
                }

                var userId = _userManager.GetUserId(context.User);
                if (userId == null)
                {
                    context.Fail();
                    return;
                }

                var participant = await _participantRepository.GetUserFromConversationAsync(new Guid(userId), new Guid(conversationId));

                if (participant.Role==Role.Admin)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
        }
    }
}
