using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Pagination;

namespace Messenger.Infrastructure.Interfaces;

public interface IConversationRepository
{
    Task<IPagedEntities<Conversation>> GetConversationsByUserIdAsync(Guid userId, int page, int pageSize);
    Task<Conversation> GetConversationByIdAsync(Guid conversationId);
    Task<Conversation> GetGroupConversationByIdAsync(Guid conversationId);
    Task<Conversation> CreateConversationWithUserAsync(User creatorUser, User user);
    Task<Conversation> GetPrivateConversationWithUserAsync(Guid creatorUserId, Guid userId);
    Task<Conversation> CreateGroupConversationAsync(string title, Guid creatorId);
    Task<Conversation> DeleteConversationAsync(Guid conversationId);
}
