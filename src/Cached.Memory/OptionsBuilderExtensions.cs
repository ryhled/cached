namespace Cached.Memory
{
    using System;
    using Configuration;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Provides support for Cached memory caching.
    /// </summary>
    public static class OptionsBuilderExtensions
    {
        /// <summary>
        ///     Creates a new CachedService object for use with the cached configurator.
        /// </summary>
        /// <param name="builder">The configurator object used during application startup.</param>
        /// <param name="options">(Optional) Service specific settings (which overrides the global settings).</param>
        /// <returns>A new cached service instance.</returns>
        public static void AddMemoryCaching(
            this CachedOptionsBuilder builder,
            MemoryCacheEntryOptions options = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddService(services => services.AddMemoryCache()); // Ensure MemoryCache is available.
            builder.AddService(
                services => services.AddSingleton(
                    provider => MemoryCacher.New(provider.GetService<IMemoryCache>(), options)));
        }

        //public static void AddMemoryCachedFunction<TResponse, TParam>(
        //    this ICachedOptions options,
        //    Func<TParam, string> keyFactory,
        //    Func<IResolver, string, TParam, Task<TResponse>> fetchFactory)
        //{
        //    if (options == null)
        //    {
        //        throw new ArgumentNullException(nameof(options));
        //    }

        //    if (keyFactory == null)
        //    {
        //        throw new ArgumentNullException(nameof(keyFactory));
        //    }

        //    if (fetchFactory == null)
        //    {
        //        throw new ArgumentNullException(nameof(fetchFactory));
        //    }

        //    options.TryAddTransient<ICached<TResponse, TParam>>(resolver => new Cached<TResponse, TParam>(
        //        resolver.GetService<IMemoryCacher>(),
        //        keyFactory,
        //        (key, arg) => fetchFactory.Invoke(resolver, key, arg)));
        //}
    }
}