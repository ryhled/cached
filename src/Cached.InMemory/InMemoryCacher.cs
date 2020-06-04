namespace Cached.InMemory
{
    using Caching;
    using Locking;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Factory for creating InMemory Cacher instances.
    /// </summary>
    public static class InMemoryCacher
    {
        /// <summary>
        /// Creates a new InMemory Cacher based on the provided MemoryCache instance.
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ICacher<IInMemory> New(IMemoryCache memoryCache, MemoryCacheEntryOptions options = null)
            => new Cacher<IInMemory>(new KeyBasedLock(), new InMemoryProvider(memoryCache, options));
    }
}
