using Cached.Caching;
using System.Threading.Tasks;

namespace Cached.Memory
{
    using System;
    using Microsoft.Extensions.Caching.Memory;

    internal sealed class MemoryCacheProvider : IMemory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _options;

        public MemoryCacheProvider(
            IMemoryCache memoryCache,
            MemoryCacheEntryOptions options)
        {
            _options = options;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public Task Set<T>(string key, T item)
        {
            _memoryCache.Set(key, item, _options);
            return Task.CompletedTask;
        }

        public Task<ValueResult<T>> TryGet<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out object dataFromCache) && dataFromCache is T castItem)
            {
                return Task.FromResult(ValueResult<T>.Hit(castItem));
            }

            return Task.FromResult(ValueResult<T>.Miss);
        }

        public Task Remove(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
