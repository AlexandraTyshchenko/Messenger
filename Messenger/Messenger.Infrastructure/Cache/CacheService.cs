using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Messenger.Infrastructure.Cache;


public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        byte[] cachedData = await _cache.GetAsync(key);

        if (cachedData != null)
        {
            var cachedDataString = Encoding.UTF8.GetString(cachedData);
            var cachedResult = JsonSerializer.Deserialize<T>(cachedDataString, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return cachedResult;
        }
        return default;
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task SetAsync<T>(string key, T objectToCache)
    {
        var serializedParticipants = JsonSerializer.Serialize(objectToCache, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        });

        var newDataToCache = Encoding.UTF8.GetBytes(serializedParticipants);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        await _cache.SetAsync(key, newDataToCache, cacheOptions);
    }
}