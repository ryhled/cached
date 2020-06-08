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
        /// <param name="cachedOptions">Options for configuring cached.</param>
        public static void AddCached(this IServiceCollection services, Action<CachedOptionsBuilder> cachedOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (cachedOptions == null)
            {
                throw new ArgumentNullException(nameof(cachedOptions));
            }

            var optionsBuilder = new CachedOptionsBuilder();
            cachedOptions.Invoke(optionsBuilder);
            optionsBuilder.Build(services);
        }
    }
}