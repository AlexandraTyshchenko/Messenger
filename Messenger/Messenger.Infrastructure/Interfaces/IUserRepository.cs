using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Pagination;

namespace Messenger.Infrastructure.Interfaces;

public interface IUserRepository
{
    Task<IPagedEntities<User>> GetUsersAsync(string userName, int page, int pageSize);//todo cash
    Task<IEnumerable<User>> GetUsersByIdsAsync(Guid[] userIds);
    Task<User> GetUserByIdAsync(Guid userId);
}
