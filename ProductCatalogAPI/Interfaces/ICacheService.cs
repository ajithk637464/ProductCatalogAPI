namespace ProductCatalogAPI.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<bool> SetIfNotExistsAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<IEnumerable<string>> GetKeysAsync(string pattern = "*");
        Task ClearAllAsync();
        Task<long> IncrementAsync(string key, long value = 1);
        Task<long> DecrementAsync(string key, long value = 1);
        Task<TimeSpan?> GetTimeToLiveAsync(string key);
        Task<bool> ExtendExpiryAsync(string key, TimeSpan additionalTime);
        Task<Dictionary<string, string>> GetCacheInfoAsync();
    }
}
