namespace Cached.Net
{
    using System;
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
        /// <param name="options">Options for configuring cached.</param>
        public static void AddCached(this IServiceCollection services, Action<ICachedConfigurationBuilder> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var config = new CachedConfigurationBuilder();
            options.Invoke(config);
            config.Build(services);
        }
    }
}