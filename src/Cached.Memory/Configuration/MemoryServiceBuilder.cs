namespace Cached.Memory.Configuration
{
    using System;
    using Cached.Configuration;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    ///     Handles configuration of Memory based Cached service.
    /// </summary>
    public sealed class MemoryServiceBuilder : ServiceBuilder<IMemory>
    {
        internal MemoryServiceBuilder()
        {
        }

        /// <summary>
        ///     MemoryCache entry options that overrides the global MemoryCAche settings.
        /// </summary>
        public MemoryCacheEntryOptions Options { get; set; }

        internal override Action<IServiceCollection> GetBuild()
        {
            return services =>
            {
                services.AddMemoryCache();
                services.AddSingleton(provider => MemoryCacheHandler.New(provider.GetService<IMemoryCache>(), Options));

                Descriptors.ForEach(services.TryAdd);
            };
        }
    }
}