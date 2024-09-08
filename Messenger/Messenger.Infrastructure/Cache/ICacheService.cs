namespace Messenger.Infrastructure.Cache;

public interface ICacheService
{
    Task RemoveAsync(string key);
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T objectToCache);
}

