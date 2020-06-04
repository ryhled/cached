using System;

namespace Cached.Caching
{
    using System.Threading.Tasks;
    using Locking;

    internal class Cacher<TProvider> : ICacher<TProvider> where TProvider: ICacheProvider
    {
        private readonly ILock _cacherLock;
        private readonly TProvider _cacheProvider;

        public Cacher(ILock cacheLock, TProvider cacheProvider)
        {
            _cacherLock = cacheLock ?? throw new ArgumentNullException(nameof(cacheLock));
            _cacheProvider = cacheProvider != null ? cacheProvider : throw new ArgumentNullException(nameof(cacheProvider));
        }

        public async Task<TResponse> GetOrFetchAsync<TResponse>(
            string key,
            Func<string, Task<TResponse>> fetchFactory)
        {
            if (fetchFactory == null)
            {
                throw new ArgumentNullException(nameof(fetchFactory));
            }

            var prefixedCacheKey = $"{nameof(Cacher<TProvider>)}|{typeof(TResponse).FullName}|{key}";

            if (await _cacheProvider.TryGetFromCache(prefixedCacheKey, out TResponse cachedData).ConfigureAwait(false))
            {
                return cachedData;
            }

            return await FetchAndAddToCache(prefixedCacheKey, () => fetchFactory(key)).ConfigureAwait(false);
        }

        private async Task<TResponse> FetchAndAddToCache<TResponse>(
            string prefixedCacheKey,
            Func<Task<TResponse>> fetchFactory)
        {
            using (await _cacherLock.LockAsync(prefixedCacheKey).ConfigureAwait(false))
            {
                if (await _cacheProvider.TryGetFromCache(prefixedCacheKey, out TResponse cachedData).ConfigureAwait(false))
                {
                    return cachedData;
                }

                TResponse data = await fetchFactory().ConfigureAwait(false);
                await _cacheProvider.WriteToCache(prefixedCacheKey, data).ConfigureAwait(false);
                return data;
            }
        }
    }
}
