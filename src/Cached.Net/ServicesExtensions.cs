namespace Cached.Net
{
    using System;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Cached net core and net 5+ service integration extensions.
    /// </summary>
    public static class ServicesExtensions
    {
        /// <summary>
        ///     Adds support for caching using Cached.
        /// </summary>
        /// <param name="services">The target service collection.</param>
        /// <param name="config">Options for configuring cached.</param>
        public static void AddCached(this IServiceCollection services, Action<ICachedConfigurationBuilder> config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var builder = new CachedConfigurationBuilder();
            config.Invoke(builder);
            builder.Build(services);
        }
    }
}