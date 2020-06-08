namespace Cached.Memory
{
    using System;
    using Cached.Configuration;
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
        /// <param name="memoryOptions">(Optional) Service specific settings (which overrides the global settings).</param>
        /// <returns>A new cached service instance.</returns>
        public static void AddMemoryCaching(
            this CachedOptionsBuilder builder,
            Action<MemoryOptions> memoryOptions = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var options = new MemoryOptions();
            memoryOptions?.Invoke(options);

            options.Services.ForEach(builder.AddService);

            builder.AddService(services => services.AddMemoryCache());
            builder.AddService(
                services => services.AddSingleton(
                    provider => MemoryCacher.New(provider.GetService<IMemoryCache>(), options.Options)));
        }
    }
}