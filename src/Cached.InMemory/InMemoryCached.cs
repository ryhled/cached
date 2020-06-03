namespace Cached.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Caching;

    /// <inheritdoc cref="ICached{TResponse,TParam}" />
    public sealed class InMemoryCached<TResponse, TParam> : ICached<TResponse, TParam>
    {
        private readonly Func<string, TParam, Task<TResponse>> _fetchFactory;
        private readonly Func<TParam, string> _keyFactory;
        private readonly ICacher<InMemory> _cacher;

        /// <summary>
        ///     Creates a new Cached instance for a specific, predefined, fetch factory function.
        /// </summary>
        /// <param name="cacher">The memory cache to be used.</param>
        /// <param name="keyFactory">The key factory that will be used to generate cache key.</param>
        /// <param name="fetchFactory">The fetch factory that is used to fetch new data.</param>
        public InMemoryCached(
            ICacher<InMemory> cacher,
            Func<TParam, string> keyFactory,
            Func<string, TParam, Task<TResponse>> fetchFactory)
        {
            _cacher = cacher ?? throw new ArgumentNullException(nameof(cacher));
            _fetchFactory = fetchFactory ?? throw new ArgumentNullException(nameof(fetchFactory));
            _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        }

        /// <inheritdoc />
        public async Task<TResponse> GetOrFetchAsync(TParam arg)
        {
            return await _cacher.GetOrFetchAsync(_keyFactory(arg), key => _fetchFactory(key, arg));
        }
    }
}