using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Messenger.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _applicationContext;

    public UserRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _applicationContext.FindAsync<User>(userId);
    }

    public async Task<IPagedEntities<User>> GetUsersAsync(string userName, int page, int pageSize)
    {
        IQueryable<User> users = _applicationContext.Users;

        if (!string.IsNullOrEmpty(userName))
        {
            users = users.Where(x => x.UserName.Contains(userName));
        }

        IPagedEntities<User> pagedUsers = await users.WithPagingAsync(page, pageSize);

        return pagedUsers;
    }

    public async Task<IEnumerable<User>> GetUsersByIdsAsync(Guid[] userIds)
    {
        return await _applicationContext.Users
                                 .Where(user => userIds.Contains(user.Id))
                                 .ToListAsync();
    }
}
