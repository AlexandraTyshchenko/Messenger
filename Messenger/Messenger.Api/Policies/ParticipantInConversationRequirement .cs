using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Api.Policies
{
    public class ParticipantInConversationRequirement : IAuthorizationRequirement
    {
        public string ConversationId { get; }

        public ParticipantInConversationRequirement(string conversationId = "")
        {
            ConversationId = conversationId;
        }
    }

    public class ParticipantInConversationAuthorizationHandler : AuthorizationHandler<ParticipantInConversationRequirement>
    {
        private readonly UserManager<User> _userManager;
        private readonly IParticipantRepository _participantRepository;

        public ParticipantInConversationAuthorizationHandler(UserManager<User> userManager, IParticipantRepository participantRepository)
        {
            _userManager = userManager;
            _participantRepository = participantRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ParticipantInConversationRequirement requirement)
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

            var isParticipant = await _participantRepository.IsUserInConversationAsync(new Guid(userId), new Guid(conversationId));

            if (isParticipant)
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
