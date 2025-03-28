using Microsoft.Extensions.Caching.Memory;

namespace RadiusCore
{
    class SessionCache
    {

        MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public void Set<T>(string key, T value)
        {
            //设置过期时间
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));
            _cache.Set(key, value, cacheEntryOptions);
        }

        public T? Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public bool Contains(string key)
        {
            return _cache.TryGetValue(key, out _);
        }
    }
}
