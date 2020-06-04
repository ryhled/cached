namespace Cached.InMemory
{
    using System;
    using Caching;
    using Locking;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    ///     Factory for creating InMemory Cacher instances.
    /// </summary>
    public static class InMemoryCacher
    {
        private static readonly Lazy<IMemoryCache> LazyMemoryCache = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));

        /// <summary>
        ///     Creates a new InMemory Cacher based on an internal, globally shared, default MemoryCache instance.
        /// </summary>
        /// <param name="options">(Optional) Provide cache options that will be applied to all entries for this cacher.</param>
        /// <returns>New InMemory Cacher instance.</returns>
        public static ICacher<IInMemory> Default(MemoryCacheEntryOptions options = null)
            => New(LazyMemoryCache.Value, options);

        /// <summary>
        ///     Creates a new InMemory Cacher based on the provided MemoryCache instance.
        /// </summary>
        /// <param name="memoryCache">The MemoryCache instance to be used with this cacher.</param>
        /// <param name="options">(Optional) Provide cache options that will be applied to all entries for this cacher.</param>
        /// <returns>New InMemory Cacher instance.</returns>
        public static ICacher<IInMemory> New(IMemoryCache memoryCache, MemoryCacheEntryOptions options = null)
            => new Cacher<IInMemory>(new KeyBasedLock(), new InMemoryProvider(memoryCache, options));
    }
}
