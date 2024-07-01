using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync(string userName);
    Task<IEnumerable<User>> GetUsersByIdsAsync(Guid[] userIds);
    Task<User> GetUserByIdAsync(Guid userId);
}
