
namespace Cached.Caching
{
    using System.Threading.Tasks;

    public interface ICacheProvider
    {
        Task WriteToCache<T>(string key, T data);
        Task<bool> TryGetFromCache<T>(string key, out T cachedItem);
    }
}
