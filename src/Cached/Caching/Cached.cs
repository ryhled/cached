namespace Cached.Caching
{
    using System;
    using System.Threading.Tasks;

    internal sealed class Cached<TValue, TParam, TProvider> : ICached<TValue, TParam> where TProvider : ICacheProvider
    {
        private readonly ICache<TProvider> _cache;
        private readonly Func<string, TParam, Task<TValue>> _fetchFactory;
        private readonly Func<TParam, string> _keyFactory;

        public Cached(
            ICache<TProvider> cache,
            Func<TParam, string> keyFactory,
            Func<string, TParam, Task<TValue>> fetchFactory)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _fetchFactory = fetchFactory ?? throw new ArgumentNullException(nameof(fetchFactory));
            _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        }

        public async Task<TValue> GetOrFetchAsync(TParam arg)
        {
            return await _cache.GetOrFetchAsync(_keyFactory(arg), key => _fetchFactory(key, arg));
        }
    }
}