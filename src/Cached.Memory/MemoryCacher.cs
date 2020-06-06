namespace Cached.Memory
{
    using System;
    using System.Threading.Tasks;
    using Caching;
    using Locking;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    ///     Factory for creating InMemory Cacher instances.
    /// </summary>
    public class MemoryCacher : Cacher, IMemoryCacher
    {
        private static readonly Lazy<IMemoryCache> LazyMemoryCache =
            new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));

        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _options;

        /// <inheritdoc />
        public MemoryCacher(
            ILock cacheClock,
            IMemoryCache memoryCache,
            MemoryCacheEntryOptions options)
            : base(cacheClock)
        {
            _options = options;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>
        ///     Creates a new InMemory Cacher based on an internal, globally shared, default MemoryCache instance.
        /// </summary>
        /// <param name="options">(Optional) Provide cache options that will be applied to all entries for this cacher.</param>
        /// <returns>New InMemory Cacher instance.</returns>
        public static IMemoryCacher Default(MemoryCacheEntryOptions options = null)
        {
            return New(LazyMemoryCache.Value, options);
        }

        /// <summary>
        ///     Creates a new InMemory Cacher based on the provided MemoryCache instance.
        /// </summary>
        /// <param name="memoryCache">The MemoryCache instance to be used with this cacher.</param>
        /// <param name="options">(Optional) Provide cache options that will be applied to all entries for this cacher.</param>
        /// <returns>New InMemory Cacher instance.</returns>
        public static IMemoryCacher New(IMemoryCache memoryCache, MemoryCacheEntryOptions options = null)
        {
            return new MemoryCacher(new KeyBasedLock(), memoryCache, options);
        }

        /// <inheritdoc />
        protected override Task WriteToCache<T>(string key, T item)
        {
            _memoryCache.Set(key, item, _options);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task<CacheResult<T>> TryGetFromCache<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out object dataFromCache) && dataFromCache is T castItem)
            {
                return Task.FromResult(CacheResult<T>.Hit(castItem));
            }

            return Task.FromResult(CacheResult<T>.Miss);
        }
    }
}