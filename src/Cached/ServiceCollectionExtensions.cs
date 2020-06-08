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
        /// <param name="options">Options for configuring cached.</param>
        public static void AddCached(this IServiceCollection services, Action<CachedOptionsBuilder> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var builder = new CachedOptionsBuilder();
            options.Invoke(builder);
            builder.Build(services);
        }
    }
}