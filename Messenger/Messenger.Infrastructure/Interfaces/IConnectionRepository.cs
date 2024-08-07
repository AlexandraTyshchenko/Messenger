using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces;

public interface IConnectionRepository
{
    Task AddConnectionAsync(User user, string connectionId);
    Task<IEnumerable<UserConnection>> GetUsersConnectionsAsync(Guid[] userIds);
    Task<IEnumerable<UserConnection>> GetUserConnectionsAsync(Guid userId);
    Task<IEnumerable<string>> GetUserConversationConnections(Guid userId);
    Task<UserConnection> RemoveConnectionAsync(string connectionId);
}
