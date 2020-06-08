namespace Cached
{
    using System;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Cached net core and net 5+ service integration extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds support for caching using Cached.
        /// </summary>
        /// <param name="services">The target service collection.</param>
        /// <param name="configuration">Configuration for initializing Cached.</param>
        public static void AddCached(this IServiceCollection services, Action<CachedConfiguration> configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var builtConfiguration = new CachedConfiguration();
            configuration.Invoke(builtConfiguration);

            if (builtConfiguration.Services.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cached configuration was empty. At least one cache service must be configured.");
            }

            builtConfiguration.Services
                .ForEach(c => c(services));
        }
    }
}