using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Pagination;

namespace Messenger.Infrastructure.Interfaces;

public interface IUserRepository
{
    Task<IPagedEntities<User>> GetUsersAsync(string userName, int page, int pageSize);
    Task<IEnumerable<User>> GetUsersByIdsAsync(Guid[] userIds);
    Task<User> GetUserByIdAsync(Guid userId);
}
