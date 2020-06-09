namespace Cached.Memory
{
    using System;
    using Cached.Configuration;
    using Configuration;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Provides support for Cached using MemoryCache.
    /// </summary>
    public static class OptionsBuilderExtensions
    {
        /// <summary>
        ///     Creates a new CachedService object for use with the cached configurator.
        /// </summary>
        /// <param name="configuration">The configurator object used during application startup.</param>
        /// <param name="options">(Optional) Service specific settings (which overrides the global settings).</param>
        /// <returns>A new cached service instance.</returns>
        public static void AddMemoryCaching(
            this CachedConfiguration configuration,
            Action<MemoryOptions> options = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var builtOptions = new MemoryOptions();
            options?.Invoke(builtOptions);

            configuration.Services.Add(services => services.AddMemoryCache());

            configuration.Services.Add(
                services => services.AddSingleton(
                    provider => MemoryCacheHandler.New(provider.GetService<IMemoryCache>(), builtOptions.Options)));

            builtOptions.Services.ForEach(configuration.Services.Add);
        }
    }
}