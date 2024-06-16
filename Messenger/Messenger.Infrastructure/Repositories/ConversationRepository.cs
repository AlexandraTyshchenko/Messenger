using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly ApplicationContext _applicationContext;

        public ConversationRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(Guid userId)
        {
            return await _applicationContext.Conversations
                 .Where(x => x.ParticipantsInConversation.Any(x => x.User.Id == userId))
                 .Include(x => x.Group)
                 .Include(x => x.Messages)
                     .ThenInclude(x => x.Sender)
                 .ToListAsync();
        }
    }
}
