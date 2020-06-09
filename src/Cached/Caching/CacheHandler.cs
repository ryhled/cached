namespace Cached.Caching
{
    using System;
    using System.Threading.Tasks;
    using Locking;

    internal sealed class CacheHandler<TProvider> : ICache<TProvider> where TProvider : ICacheProvider
    {
        private readonly ILock _cacheLock;

        public CacheHandler(
            TProvider cacheProvider, 
            ILock cacheLock)
        {
            Provider = cacheProvider != null ? cacheProvider : throw new ArgumentNullException(nameof(cacheProvider));
            _cacheLock = cacheLock ?? throw new ArgumentNullException(nameof(cacheLock));
        }

        public TProvider Provider { get; }

        public async Task<TValue> GetOrFetchAsync<TValue>(
            string key,
            Func<string, Task<TValue>> fetchFactory)
        {
            if (fetchFactory == null)
            {
                throw new ArgumentNullException(nameof(fetchFactory));
            }

            var result = await Provider.TryGet<TValue>(key).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return result.Value;
            }

            return await FetchAndAddToCache(key, fetchFactory);
        }

        private async Task<TValue> FetchAndAddToCache<TValue>(
            string key,
            Func<string, Task<TValue>> fetchFactory)
        {
            using (await _cacheLock.LockAsync(key).ConfigureAwait(false))
            {
                var result = await Provider.TryGet<TValue>(key).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return result.Value;
                }

                TValue value = await fetchFactory(key).ConfigureAwait(false);
                await Provider.Set(key, value).ConfigureAwait(false);
                return value;
            }
        }
    }
}
