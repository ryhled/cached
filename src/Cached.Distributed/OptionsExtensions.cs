namespace Cached.Distributed
{
    using System;
    using System.Threading.Tasks;
    using Caching;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Provides the CachedConfigurator with an Distributed service instance.
    /// </summary>
    public static class OptionsExtensions
    {
        /// <summary>
        ///     Creates a new CachedService object for use with the cached configurator.
        /// </summary>
        /// <param name="builder">The configurator object used during application startup.</param>
        /// <param name="options">(Optional) Service specific settings (which overrides the global settings).</param>
        /// <returns>A new cached service instance.</returns>
        public static void AddDistributedCaching(
            this ICachedConfigurationBuilder builder,
            DistributedCacheEntryOptions options = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.TryAddSingleton(provider => DistributedCacher.New(provider.GetService<IDistributedCache>(), options));
        }

        /// <summary>
        ///     Adds a cache-wrapped function as an injectable instance.
        /// </summary>
        /// <typeparam name="TResponse">The type of data we want to query for.</typeparam>
        /// <typeparam name="TParam">The type of object we want to query from.</typeparam>
        /// <param name="options">The Cached OptionsBuilder, responsible for putting everything together.</param>
        /// <param name="keyFactory">A function that specifies how to construct the cache key out of the passed parameter.</param>
        /// <param name="fetchFactory">A function which specifies how to fetch the data if it does not exist in cache.</param>
        public static void AddDistributedCachedFunction<TResponse, TParam>(
            this ICachedConfigurationBuilder options,
            Func<TParam, string> keyFactory,
            Func<IResolver, string, TParam, Task<TResponse>> fetchFactory)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (keyFactory == null)
            {
                throw new ArgumentNullException(nameof(keyFactory));
            }

            if (fetchFactory == null)
            {
                throw new ArgumentNullException(nameof(fetchFactory));
            }

            options.TryAddTransient(resolver => new Cached<TResponse, TParam>(
                resolver.GetService<IDistributedCacher>(),
                keyFactory,
                (key, arg) => fetchFactory.Invoke(resolver, key, arg)));
        }
    }
}
