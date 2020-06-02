namespace Cached.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Caching;
    using Locking;
    using Microsoft.Extensions.Caching.Memory;

    /// <inheritdoc cref="IInMemoryCacher" />
    public sealed class InMemoryCacher : Cacher, IInMemoryCacher
    {
        private static readonly Lazy<IMemoryCache> DefaultInstance =
            new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));

        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _options;

        internal InMemoryCacher(
            IMemoryCache memoryCache,
            ILock cacherLock,
            MemoryCacheEntryOptions options)
            : base(cacherLock)
        {
            _options = options;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>
        ///     Creates a default InMemoryCacher instance, using a globally shared, long lived, MemoryCache instance.
        /// </summary>
        /// <param name="options">(Optional) Customized Cached settings.</param>
        /// <returns>A new InMemoryCacher instance.</returns>
        public static InMemoryCacher Default(MemoryCacheEntryOptions options = null)
        {
            return New(DefaultInstance.Value, options);
        }

        /// <summary>
        ///     Create a new MemoryCacher instance.
        /// </summary>
        /// <param name="memoryCacher">The underlying MemoryCacher instance to be used.</param>
        /// <param name="options">(Optional) Customized Cached options.</param>
        /// <returns>A new InMemoryCacher instance.</returns>
        public static InMemoryCacher New(IMemoryCache memoryCacher, MemoryCacheEntryOptions options = null)
        {
            return new InMemoryCacher(
                memoryCacher ?? throw new ArgumentNullException(nameof(memoryCacher)),
                new KeyBasedLock(),
                options);
        }

        /// <summary>
        ///     <para>Tries to get data from the underlying MemoryCache.</para>
        ///     <para>
        ///         Since MemoryCache itself is thread-safe, we do not need to lock read operations.
        ///         <see href="https://docs.microsoft.com/en-us/dotnet/api/system.runtime.caching.memorycache">
        ///             MemoryCache
        ///             Documentation
        ///         </see>
        ///     </para>
        /// </summary>
        /// <typeparam name="T">The type of data requested.</typeparam>
        /// <param name="key">The cache key that represents the data.</param>
        /// <param name="cachedItem">The data retrieved from the cache.</param>
        /// <returns>A boolean, representing if the data was found, or not.</returns>
        protected override Task<bool> TryGetFromCache<T>(
            string key,
            out T cachedItem)
        {
            if (_memoryCache.TryGetValue(key, out object dataFromCache) && dataFromCache is T castData)
            {
                cachedItem = castData;
                return Task.FromResult(true);
            }

            cachedItem = default;
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        protected override Task WriteToCache<T>(
            string key,
            T data)
        {
            _memoryCache.Set(key, data, _options);
            return Task.CompletedTask;
        }
    }
}