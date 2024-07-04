using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces;

public interface IConversationRepository
{
    Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(Guid userId);
    Task<Conversation> GetConversationByIdAsync(Guid conversationId);
    Task<Conversation> GetGroupConversationByIdAsync(Guid conversationId);
    Task<Conversation> CreateConversationWithUserAsync(User creatorUser, User user);
    Task<Conversation> GetPrivateConversationWithUserAsync(Guid creatorUserId, Guid userId);
    Task<Conversation> CreateGroupConversationAsync(string title, string imgUrl, Guid creatorId);
    Task<Conversation> DeleteConversationAsync(Guid conversationId);
}
