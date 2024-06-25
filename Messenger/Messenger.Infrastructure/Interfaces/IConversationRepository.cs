using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IConversationRepository
    {
        Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(Guid userId);
        Task<Conversation> GetConversationByIdAsync(Guid conversationId);
        Task CreatePrivateConversationWithUserAsync(Guid creatorUserId, Guid userId);
        Task CreateGroupConversationAsync(string title, string imgUrl, Guid creatorId);
        Task<Conversation> DeleteConversationAsync(Guid conversationId);
    }
}
