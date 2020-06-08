namespace Cached.Distributed
{
    using System;
    using System.Threading.Tasks;
    using Caching;
    using Locking;
    using Microsoft.Extensions.Caching.Distributed;

    /// <inheritdoc cref="IDistributedCacher" />
    public sealed class DistributedCacher : Cacher, IDistributedCacher
    {
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions _options;

        internal DistributedCacher(
            ILock cacheLock,
            IDistributedCache distributedCache,
            DistributedCacheEntryOptions options = null)
            : base(cacheLock)
        {
            _options = options;
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        /// <summary>
        ///     Creates a new Cacher instance backed by provided DistributedCache instance.
        /// </summary>
        /// <param name="distributedCache">The DistributedCache instance to be used.</param>
        /// <param name="options">(Optional) Entry options that overrides the DistributedCache configuration.</param>
        /// <returns>The new cacher instance.</returns>
        public static IDistributedCacher New(
            IDistributedCache distributedCache,
            DistributedCacheEntryOptions options = null)
        {
            return new DistributedCacher(new KeyBasedLock(), distributedCache, options);
        }

        /// <inheritdoc />
        protected override async Task WriteToCache<T>(string key, T item)
        {
            await _distributedCache.SetAsync(key, item.ToByteArray()).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override async Task<CacheResult<T>> TryGetFromCache<T>(string key)
        {
            var itemBytes = await _distributedCache.GetAsync(key).ConfigureAwait(false);

            return itemBytes.TryParseObject(out T castItem)
                ? CacheResult<T>.Hit(castItem)
                : CacheResult<T>.Miss;
        }
    }
}