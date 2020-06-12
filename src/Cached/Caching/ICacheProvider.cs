namespace Cached.Caching
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     Implements a caching scheme for use with a cache handler.
    /// </summary>
    public interface ICacheProvider : IDisposable
    {
        /// <summary>
        ///     Writes the data to cache.
        /// </summary>
        /// <typeparam name="TValue">The type of value to be written.</typeparam>
        /// <param name="key">The cache key to be used for the data.</param>
        /// <param name="value">The value that is to be persisted in cache.</param>
        /// <returns></returns>
        Task Set<TValue>(string key, TValue value);

        /// <summary>
        ///     Tries to retrieve data from cache based on the provided key.
        /// </summary>
        /// <typeparam name="TValue">The type of the item being retrieved.</typeparam>
        /// <param name="key">The key to use when trying to locate data.</param>
        /// <returns>True if item is found, otherwise false.</returns>
        Task<CachedValueResult<TValue>> TryGet<TValue>(string key);

        /// <summary>
        ///     Attempts to remove an item from the cache.
        /// </summary>
        /// <param name="key">The key that represents the item to be removed.</param>
        /// <returns></returns>
        Task Remove(string key);
    }
}