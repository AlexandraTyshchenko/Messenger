using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories;

public class ConnectionRepository : IConnectionRepository
{
    private readonly ApplicationContext _applicationContext;

    public ConnectionRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task AddConnectionAsync(User user, string connectionId)
    {
        var connection = new UserConnection
        {
            ConnectionId = connectionId,
            User = user
        };

        await _applicationContext.UserConnections.AddAsync(connection);
    }

    public async Task<IEnumerable<UserConnection>> GetUsersConnectionsAsync(Guid[] userIds)
    {
        return await _applicationContext
            .UserConnections.Include(x => x.User).Where(x => userIds.Contains(x.User.Id)).ToListAsync();
    }

    public async Task<IEnumerable<string>> GetUserConversationConnections(Guid userId)
    {
        var conversations = await _applicationContext.Conversations
            .Where(c => c.ParticipantsInConversation.Any(p => p.User.Id == userId))
            .Select(c => c.Id.ToString())
            .ToListAsync();

        return conversations;
    }

    public async Task<IEnumerable<UserConnection>> GetUserConnectionsAsync(Guid userId)
    {
        return await _applicationContext
            .UserConnections
            .Include(x => x.User)
            .Where(x => x.User.Id == userId)
            .ToListAsync();
    }
}
