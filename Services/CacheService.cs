using Microsoft.Extensions.Caching.Memory;
using SimpleBlogMVC.Services.Interfaces;
using System;

namespace SimpleBlogMVC.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public void Set<T>(string key, T value, TimeSpan duration)
        {
            _memoryCache.Set(key, value, duration);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public bool TryGet<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }
    }
}