
namespace Cached.Caching
{
    using System.Threading.Tasks;

    /// <summary>
    ///     Responsible for performing the operations required against the cache source.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        ///     Writes the data to cache.
        /// </summary>
        /// <typeparam name="T">The type of item to be written.</typeparam>
        /// <param name="key">The cache key to be used for the data.</param>
        /// <param name="item">The item that is to be persisted in cache.</param>
        /// <returns></returns>
        Task WriteToCache<T>(string key, T item);

        /// <summary>
        /// Tries to retrieve data from cache based on the provided key.
        /// </summary>
        /// <typeparam name="T">The type of the item being searched for.</typeparam>
        /// <param name="key">The key to use when trying to locate data.</param>
        /// <param name="item">The item that was located from the cache.</param>
        /// <returns>True if item is found, otherwise false.</returns>
        Task<bool> TryGetFromCache<T>(string key, out T item);
    }
}
