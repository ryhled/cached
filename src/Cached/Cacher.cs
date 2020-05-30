namespace Cached
{
    using System;
    using System.Threading.Tasks;
    using Locking;

    /// <inheritdoc />
    public abstract class Cacher<TCatcher> : ICacher
    {
        private readonly ILock _cacherLock;

        internal Cacher(ILock cacheLock)
        {
            _cacherLock = cacheLock ?? throw new ArgumentNullException(nameof(cacheLock));
        }

        /// <inheritdoc />
        public async Task<TResponse> GetOrFetchAsync<TResponse>(
            string key,
            Func<Task<TResponse>> fetchFactory)
        {
            var prefixedCacheKey = $"{typeof(TCatcher).Name}_{typeof(TResponse).FullName}_{key}";

            if (await TryGetFromCache(prefixedCacheKey, out TResponse cachedData).ConfigureAwait(false))
            {
                return cachedData;
            }

            return await FetchAndAddToCache(prefixedCacheKey, fetchFactory).ConfigureAwait(false);
        }

        private async Task<TResponse> FetchAndAddToCache<TResponse>(
            string key,
            Func<Task<TResponse>> fetchFactory)
        {
            using (await _cacherLock.LockAsync(key).ConfigureAwait(false))
            {
                if (await TryGetFromCache(key, out TResponse cachedData).ConfigureAwait(false))
                {
                    return cachedData;
                }

                TResponse data = await fetchFactory().ConfigureAwait(false);
                await WriteToCache(key, data).ConfigureAwait(false);
                return data;
            }
        }

        /// <summary>
        ///     Writes data to cache, using the provided key as cache identifier.
        /// </summary>
        /// <typeparam name="T">The type of data requested.</typeparam>
        /// <param name="key">The cache key that will represent the data.</param>
        /// <param name="data">The data that needs to be put in cache.</param>
        /// <returns></returns>
        protected abstract Task WriteToCache<T>(string key, T data);

        /// <summary>
        ///     <para>    Try to retrieve data from cache.</para>
        ///     <para>
        ///         Implementations of this method needs to be thread-safe.
        ///         Normally cache libraries are by default, but otherwise locking might be needed.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">The type of data requested.</typeparam>
        /// <param name="key">The cache key that represents the data.</param>
        /// <param name="cachedItem">The data retrieved from the cache.</param>
        /// <returns>A boolean, representing if the data was found, or not.</returns>
        protected abstract Task<bool> TryGetFromCache<T>(string key, out T cachedItem);
    }
}