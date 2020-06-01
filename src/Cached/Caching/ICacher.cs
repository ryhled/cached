namespace Cached.Caching
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     Applies caching to expensive operations.
    /// </summary>
    public interface ICacher
    {
        /// <summary>
        ///     Tries to get data from cache.
        ///     If data does not exist, it will be fetched by executing the provided function.
        ///     Ensures that parallel request does not cause 'cache miss-storm' and that data is fetched only once.
        /// </summary>
        /// <typeparam name="TResponse">The type of data requested.</typeparam>
        /// <param name="key">The cache key</param>
        /// <param name="fetchFactory">The factory function to use for fetching new data</param>
        /// <returns>The data that corresponds to the provided cache key</returns>
        Task<TResponse> GetOrFetchAsync<TResponse>(string key, Func<Task<TResponse>> fetchFactory);
    }
}