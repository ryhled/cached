namespace Cached.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Caching;
    using Microsoft.Extensions.Caching.Memory;

    /// <inheritdoc />
    public sealed class InMemory : ICacheProvider
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _options;

        internal InMemory(
            IMemoryCache memoryCache,
            MemoryCacheEntryOptions options)
        {
            _options = options;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <inheritdoc />
        public Task WriteToCache<T>(string key, T data)
        {
            _memoryCache.Set(key, data, _options);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<bool> TryGetFromCache<T>(string key, out T cachedItem)
        {
            if (_memoryCache.TryGetValue(key, out object dataFromCache) && dataFromCache is T castData)
            {
                cachedItem = castData;
                return Task.FromResult(true);
            }

            cachedItem = default;
            return Task.FromResult(false);
        }
    }
}
