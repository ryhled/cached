namespace Cached.Configuration
{
    using System;
    using System.Collections.Generic;
    using Caching;
    using Microsoft.Extensions.DependencyInjection;

    /// <inheritdoc />
    public sealed class CachedOptionsBuilder : ICachedOptions
    {
        private readonly List<Action<IServiceCollection>> _serviceBuilder
            = new List<Action<IServiceCollection>>();


        /// <inheritdoc />
        public void AddService<TProvider>(ServiceBuilder<TProvider> builder) where TProvider : ICacheProvider
            => _serviceBuilder.Add(builder.Build);

        internal void Build(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (_serviceBuilder.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cached configuration was empty. At least one cache service must be configured.");
            }

            _serviceBuilder.ForEach(configuration => configuration(services));
        }
    }
}