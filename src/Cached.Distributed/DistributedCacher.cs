namespace Cached.Distributed
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
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
