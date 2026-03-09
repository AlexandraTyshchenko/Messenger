using Messenger.Infrastructure.Health;
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
        private readonly RedisStateService _redisState;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger, RedisStateService redisState)
        {
            _cache = cache;
            _logger = logger;
            _redisState = redisState;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (!_redisState.IsAvailable)
            {
                _logger.LogWarning("Redis unavailable. Skipping cache GET for key: {Key}", key);
                return default;
            }

            _logger.LogInformation("Getting cache for key: {Key}", key);
            var cachedData = await _cache.GetAsync(key);
            if (cachedData == null)
            {
                _logger.LogInformation("Cache miss for key: {Key}", key);
                return default;
            }
            var json = Encoding.UTF8.GetString(cachedData);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
        }

        public async Task SetAsync<T>(string key, T value)
        {
            if (!_redisState.IsAvailable)
            {
                _logger.LogWarning("Redis unavailable. Skipping cache GET for key: {Key}", key);
                return;
            }

            _logger.LogInformation("Setting cache for key: {Key}", key);

            var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });

            var data = Encoding.UTF8.GetBytes(json);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            await _cache.SetAsync(key, data, options);
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _logger.LogInformation("Removing cache for key: {Key}", key);
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache REMOVE failed for key: {Key}", key);
            }
        }
    }
}