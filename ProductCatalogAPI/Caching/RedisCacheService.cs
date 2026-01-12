using ProductCatalogAPI.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
namespace ProductCatalogAPI.Caching
{

    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value!);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await _db.StringSetAsync(key, JsonSerializer.Serialize(value), (Expiration)expiry);
        }

        public async Task<bool> SetIfNotExistsAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            return await _db.StringSetAsync(
                key,
                JsonSerializer.Serialize(value),
                expiry,
                When.NotExists
            );
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string pattern = "*")
        {
            var server = GetServer();
            return await Task.FromResult(
                server.Keys(pattern: pattern).Select(k => k.ToString())
            );
        }

        public async Task ClearAllAsync()
        {
            var server = GetServer();
            await server.FlushDatabaseAsync();
        }

        public async Task<long> IncrementAsync(string key, long value = 1)
        {
            return await _db.StringIncrementAsync(key, value);
        }

        public async Task<long> DecrementAsync(string key, long value = 1)
        {
            return await _db.StringDecrementAsync(key, value);
        }

        public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
        {
            return await _db.KeyTimeToLiveAsync(key);
        }

        public async Task<bool> ExtendExpiryAsync(string key, TimeSpan additionalTime)
        {
            var ttl = await _db.KeyTimeToLiveAsync(key);
            if (ttl == null) return false;

            return await _db.KeyExpireAsync(key, ttl.Value + additionalTime);
        }

        public async Task<Dictionary<string, string>> GetCacheInfoAsync()
        {
            var server = GetServer();
            var info = await server.InfoAsync();

            return info
                .SelectMany(section => section)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints().First();
            return _redis.GetServer(endpoint);
        }
    }

}
