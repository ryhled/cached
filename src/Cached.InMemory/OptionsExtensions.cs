namespace Cached.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Caching;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Provides the CachedConfigurator with an InMemory service instance.
    /// </summary>
    public static class OptionsExtensions
    {
        /// <summary>
        ///     Creates a new CachedService object for use with the cached configurator.
        /// </summary>
        /// <param name="builder">The configurator object used during application startup.</param>
        /// <param name="options">(Optional) Service specific settings (which overrides the global settings).</param>
        /// <returns>A new cached service instance.</returns>
        public static void AddInMemoryCaching(this ICachedConfigurationBuilder builder, MemoryCacheEntryOptions options = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddSingletonService<IInMemoryCacher, InMemoryCacher>(
                provider => InMemoryCacher.New(provider.GetService<IMemoryCache>(), options));
        }

        /// <summary>
        ///     Adds a cache-wrapped function as an injectable instance.
        /// </summary>
        /// <typeparam name="TResponse">The type of data we want to query for.</typeparam>
        /// <typeparam name="TParam">The type of object we want to query from.</typeparam>
        /// <param name="options">The Cached OptionsBuilder, responsible for putting everything together.</param>
        /// <param name="keyFactory">A function that specifies how to construct the cache key out of the passed parameter.</param>
        /// <param name="fetchFactory">A function which specifies how to fetch the data if it does not exist in cache.</param>
        public static void AddInMemoryCachedFunction<TResponse, TParam>(
            this ICachedConfigurationBuilder options,
            Func<TParam, string> keyFactory,
            Func<IServiceProvider, TParam, Task<TResponse>> fetchFactory)
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

            options.AddTransientService<ICached<TResponse, TParam>, InMemoryCached<TResponse, TParam>>(provider =>
                new InMemoryCached<TResponse, TParam>(
                    provider.GetService<IInMemoryCacher>(),
                    keyFactory,
                    param => fetchFactory.Invoke(provider, param)));
        }
    }
}