using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace Messenger.Infrastructure.Cache;
public interface ICacheService
{
    Task RemoveAsync(string key);
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T objectToCache);
}

