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
        public async Task<IEnumerable<User>> GetParticipantsByConversationIdAsync(Guid conversationId)
        {
            return await _applicationContext.ParticipantsInConversation.Where(x => x.Conversation.Id == conversationId).Include(x => x.User).Select(x => x.User).ToListAsync();
        }

        public async Task<bool> IsUserInConversationAsync(Guid userId, Guid conversationId)
        {
            var participant = await _applicationContext.ParticipantsInConversation.FirstOrDefaultAsync(x => x.User.Id == userId && x.Conversation.Id == conversationId);

            return participant != null;
        }
    }
}