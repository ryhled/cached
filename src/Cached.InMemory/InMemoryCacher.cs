namespace Cached.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Locking;
    using Microsoft.Extensions.Caching.Memory;

    /// <inheritdoc cref="IInMemoryCacher" />
    public sealed class InMemoryCacher : Cacher<InMemoryCacher>, IInMemoryCacher, ICachedService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Func<DateTimeOffset> _nowFactory;
        private readonly CachedSettings _settings;

        internal InMemoryCacher(
            IMemoryCache memoryCache,
            ILock cacherLock,
            CachedSettings settings,
            Func<DateTimeOffset> nowFactory)
            : base(cacherLock)
        {
            _settings = settings;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _nowFactory = nowFactory ?? throw new ArgumentNullException(nameof(nowFactory));
        }

        /// <summary>
        ///     Create a new MemoryCacher instance.
        /// </summary>
        /// <param name="memoryCacher">The underlying MemoryCacher instance to be used.</param>
        /// <param name="settings">(Optional) Customized Cached settings.</param>
        /// <returns></returns>
        public static InMemoryCacher New(IMemoryCache memoryCacher, CachedSettings settings = default)
        {
            return new InMemoryCacher(
                memoryCacher ?? throw new ArgumentNullException(nameof(memoryCacher)),
                new SemaphoreSlimLock(),
                settings ?? new CachedSettings(),
                () => DateTimeOffset.UtcNow);
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
            if (_settings == null)
            {
                _memoryCache.Set(key, data);
                return Task.CompletedTask;
            }

            var opts = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = _nowFactory().Add(_settings.AbsoluteExpiration),
                SlidingExpiration = _settings.SlidingExpiration
            };

            _memoryCache.Set(key, data, opts);
            return Task.CompletedTask;
        }
    }
}