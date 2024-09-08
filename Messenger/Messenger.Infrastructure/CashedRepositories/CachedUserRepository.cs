using Messenger.Infrastructure.Cache;
using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Messenger.Infrastructure.KeyBuilder;
using Messenger.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.CachedRepositories;

public class CachedUserRepository : IUserRepository
{
    private readonly IUserRepository _innerRepository;
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyBuilder _cacheKeyBuilder;
    private readonly ApplicationContext _applicationContext;

    public CachedUserRepository(IUserRepository innerRepository,
                                ICacheService cacheService,
                                ICacheKeyBuilderFactory cacheKeyBuilderFactory,
                                ApplicationContext applicationContext)
    {
        _innerRepository = innerRepository;
        _cacheService = cacheService;
        _cacheKeyBuilder = cacheKeyBuilderFactory.Create(typeof(User));
        _applicationContext = applicationContext;
    }


    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        string cacheKey = _cacheKeyBuilder
            .AppendParameter(userId)
            .Build();

        User cachedUser = await _cacheService.GetAsync<User>(cacheKey);

        if (cachedUser != null)
        {
            return cachedUser;
        }

        User user = await _innerRepository.GetUserByIdAsync(userId);

        await _cacheService.SetAsync(cacheKey, user);

        return user;
    }

    public async Task<IPagedEntities<User>> GetUsersAsync(string userName, int page, int pageSize)
    {
        string cacheKey = _cacheKeyBuilder
            .AppendParameter(userName)
            .AppendParameter(page)
            .AppendParameter(pageSize)
            .Build();

        IPagedEntities<User> cachedUsers = await _cacheService.GetAsync<PagedEntities<User>>(cacheKey);

        if (cachedUsers != null)
        {
            return cachedUsers;
        }

        IPagedEntities<User> result = await _innerRepository.GetUsersAsync(userName, page, pageSize);

        await _cacheService.SetAsync(cacheKey, result);

        return result;
    }

    public async Task<IEnumerable<User>> GetUsersByIdsAsync(Guid[] userIds)
    {
        string cacheKey = _cacheKeyBuilder
            .AppendParameter("ByIds")
            .AppendParameter(string.Join("_", userIds))
            .Build();

        IEnumerable<User> cachedUsers = await _cacheService.GetAsync<IEnumerable<User>>(cacheKey);

        if (cachedUsers != null)
        {
            return cachedUsers;
        }

        IEnumerable<User> result = await _innerRepository.GetUsersByIdsAsync(userIds);

        await _cacheService.SetAsync(cacheKey, result);

        return result;
    }
}
