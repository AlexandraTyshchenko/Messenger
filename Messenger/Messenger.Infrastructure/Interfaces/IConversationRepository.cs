using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IConversationRepository
    {
        Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(Guid userId);
    }
}
