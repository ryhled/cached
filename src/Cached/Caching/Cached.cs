namespace Cached.Caching
{
    using System;
    using System.Threading.Tasks;

    internal sealed class Cached<TResponse, TParam> : ICached<TResponse, TParam>
    {
        private readonly Func<string, TParam, Task<TResponse>> _fetchFactory;
        private readonly Func<TParam, string> _keyFactory;
        private readonly ICacher _cacher;

        public Cached(
            ICacher cacher,
            Func<TParam, string> keyFactory,
            Func<string, TParam, Task<TResponse>> fetchFactory)
        {
            _cacher = cacher ?? throw new ArgumentNullException(nameof(cacher));
            _fetchFactory = fetchFactory ?? throw new ArgumentNullException(nameof(fetchFactory));
            _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        }

        public async Task<TResponse> GetOrFetchAsync(TParam arg)
        {
            return await _cacher.GetOrFetchAsync(_keyFactory(arg), key => _fetchFactory(key, arg));
        }
    }
}