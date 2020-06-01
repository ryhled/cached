namespace Cached.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Caching;

    /// <inheritdoc cref="ICached{TResponse,TParam}" />
    public sealed class InMemoryCached<TResponse, TParam> : ICached<TResponse, TParam>, ICachedService
    {
        private readonly Func<TParam, Task<TResponse>> _fetchFactory;
        private readonly Func<TParam, string> _keyFactory;
        private readonly IInMemoryCacher _memoryCacher;

        /// <summary>
        ///     Creates a new Cached instance for a specific, predefined, fetch factory function.
        /// </summary>
        /// <param name="memoryCacher">The memory cache to be used.</param>
        /// <param name="keyFactory">The key factory that will be used to generate cache key.</param>
        /// <param name="fetchFactory">The fetch factory that is used to fetch new data.</param>
        public InMemoryCached(
            IInMemoryCacher memoryCacher,
            Func<TParam, string> keyFactory,
            Func<TParam, Task<TResponse>> fetchFactory)
        {
            _memoryCacher = memoryCacher ?? throw new ArgumentNullException(nameof(memoryCacher));
            _fetchFactory = fetchFactory ?? throw new ArgumentNullException(nameof(fetchFactory));
            _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        }

        /// <inheritdoc />
        public async Task<TResponse> GetOrFetchAsync(TParam arg)
        {
            return await _memoryCacher.GetOrFetchAsync(_keyFactory(arg), () => _fetchFactory(arg));
        }
    }
}