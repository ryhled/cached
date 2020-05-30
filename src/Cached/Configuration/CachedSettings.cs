namespace Cached.Configuration
{
    using System;

    /// <summary>
    ///     Represents the cacher settings.
    /// </summary>
    public sealed class CachedSettings
    {
        /// <summary>
        ///     The default expiration value, in minutes.
        /// </summary>
        public static readonly int DefaultAbsoluteExpiration = 20;

        /// <summary>
        ///     Creates a new settings instance.
        /// </summary>
        /// <param name="absoluteExpiration">How long a cache entry may stay in the cache.</param>
        /// <param name="slidingExpiration">How long a cache entry stays in cache if not accessed.</param>
        public CachedSettings(
            TimeSpan? absoluteExpiration = null,
            TimeSpan? slidingExpiration = null)
        {
            AbsoluteExpiration = absoluteExpiration ?? TimeSpan.FromMinutes(DefaultAbsoluteExpiration);
            SlidingExpiration = slidingExpiration;
        }

        /// <summary>
        ///     The maximum amount of time that an item can stay in cache.
        /// </summary>
        public TimeSpan AbsoluteExpiration { get; }

        /// <summary>
        ///     The maximum amount of time an item may stay in cache without having been accessed.
        /// </summary>
        public TimeSpan? SlidingExpiration { get; }
    }
}