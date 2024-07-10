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

    public async Task<IEnumerable<UserConnection>> GetUserConnectionsAsync(Guid[] userIds)
    {
        return await _applicationContext
            .UserConnections.Include(x => x.User).Where(x => userIds.Contains(x.User.Id)).ToListAsync();
    }
}
