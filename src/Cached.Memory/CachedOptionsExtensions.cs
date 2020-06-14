namespace Cached.MemoryCache
{
    using System;
    using Cached.Configuration;
    using Configuration;

    /// <summary>
    ///     Provides support for Cached using MemoryCache.
    /// </summary>
    public static class CachedOptionsExtensions
    {
        /// <summary>
        ///     Creates a new CachedService object for use with the cached configurator.
        /// </summary>
        /// <param name="options">The configurator object used during application startup.</param>
        /// <param name="builder">(Optional) Service builder, used to configure different aspects of the Cached service.</param>
        public static void AddMemoryCaching(
            this ICachedOptions options,
            Action<MemoryCacheServiceBuilder> builder = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var builtConfiguration = new MemoryCacheServiceBuilder();
            builder?.Invoke(builtConfiguration);
            options.AddService(builtConfiguration);
        }
    }
}