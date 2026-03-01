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

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache GET failed for key: {Key}", key);
                return default; 
            }
        }

        public async Task SetAsync<T>(string key, T value)
        {
            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache SET failed for key: {Key}", key);
            }
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