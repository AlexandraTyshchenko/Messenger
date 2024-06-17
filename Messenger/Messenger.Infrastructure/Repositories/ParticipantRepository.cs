using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationContext _applicationContext;

        public ParticipantRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task AddParticipantsToConversationAsync(Guid[] userIds, Guid conversationId)
        {
            var users = await _applicationContext.Users
                                        .Where(user => userIds.Contains(user.Id))
                                        .ToListAsync();

            var conversation = await _applicationContext.Conversations.FindAsync(conversationId);

            if (conversation == null) {
                throw new ArgumentException("conversation not found");    
            }

            var participants = users.Select(x => new ParticipantInConversation {
                User = x,
                JoinedAt = DateTime.UtcNow,
                Conversation = conversation,
                Role = Enums.Role.BasicUser,
            });

            await _applicationContext.AddRangeAsync(participants);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetParticipantsByConversationIdAsync(Guid conversationId)
        {
            return await _applicationContext.ParticipantsInConversation.Where(x => x.Conversation.Id == conversationId)
                .Include(x => x.User)
                .Select(x => x.User)
                .ToListAsync();
        }

        public async Task<ParticipantInConversation> GetUserFromConversationAsync(Guid userId, Guid conversationId)
        {
            var participant = await _applicationContext.ParticipantsInConversation
                .FirstOrDefaultAsync(x => x.User.Id == userId && x.Conversation.Id == conversationId);

            if (participant == null)
            {
                throw new ArgumentException("participant not found");
            }

            return participant;
        }

        public async Task<bool> IsUserInConversationAsync(Guid userId, Guid conversationId)
        {
            var participant = await _applicationContext.ParticipantsInConversation
                .FirstOrDefaultAsync(x => x.User.Id == userId && x.Conversation.Id == conversationId);

            return participant != null;
        }
    }
}