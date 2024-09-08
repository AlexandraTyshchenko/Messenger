using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Messenger.Infrastructure.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            _logger.LogInformation($"Attempting to retrieve cache for key: {key}");

            byte[] cachedData = await _cache.GetAsync(key);

            if (cachedData != null)
            {
                var cachedDataString = Encoding.UTF8.GetString(cachedData);
                var cachedResult = JsonSerializer.Deserialize<T>(cachedDataString, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                });

                _logger.LogInformation($"Successfully retrieved cache for key: {key}");

                return cachedResult;
            }

            _logger.LogInformation($"Cache not found for key: {key}");

            return default;
        }

        public async Task RemoveAsync(string key)
        {
            _logger.LogInformation($"Attempting to remove cache for key: {key}");

            await _cache.RemoveAsync(key);

            _logger.LogInformation($"Cache removed for key: {key}");
        }

        public async Task SetAsync<T>(string key, T objectToCache)
        {
            _logger.LogInformation($"Attempting to set cache for key: {key}");

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

            _logger.LogInformation($"Cache successfully set for key: {key}");
        }
    }
}
