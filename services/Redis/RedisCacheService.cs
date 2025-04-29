using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;


namespace foodOrderingApp.services.Redis
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connection;
        public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer connection)
        {
            _cache = cache;
            _connection = connection;
        }

        private bool IsRedisConnected()
        {
            return _connection.IsConnected;
        }
        public T Get<T>(string key)
        {
            if (!IsRedisConnected())
            {
                return default!;
            }

            var data = _cache.GetString(key);
            if (data != null)
            {
                return JsonSerializer.Deserialize<T>(data)!;
            }
            return default!;
        }

        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            if (!IsRedisConnected())
            {
                return;
            }

            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl };
            _cache.SetString(key, JsonSerializer.Serialize(value), options);

        }

        public void Delete(string key)
        {
            if (!IsRedisConnected())
            {
                return;
            }
            _cache.Remove(key);
        }
    }
}