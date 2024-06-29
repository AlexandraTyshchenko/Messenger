using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories
{
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

        public async Task<IEnumerable<User>> GetUsersAsync(string userName)
        {
            var users = await _applicationContext.Users
                .Where(x => x.UserName.Contains(userName))
                .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<User>> GetUsersByIdsAsync(Guid[] userIds)
        {
            return await _applicationContext.Users
                                     .Where(user => userIds.Contains(user.Id))
                                     .ToListAsync();
        }
    }
}
