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
        /// <param name="options">Options for configuring cached.</param>
        public static void AddCached(this IServiceCollection services, Action<ICachedOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var config = new CachedOptionsBuilder();
            options.Invoke(config);
            config.Build(services);
        }
    }
}