namespace Cached.Configuration
{
    using System;
    using System.Collections.Generic;
    using Caching;
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class CachedOptionsBuilder : ICachedOptions
    {
        private readonly List<Action<IServiceCollection>> _serviceBuilders
            = new List<Action<IServiceCollection>>();

        public void AddService<TProvider>(ServiceBuilder<TProvider> builder)
            where TProvider : ICacheProvider
            => _serviceBuilders.Add(builder.GetBuild());

        internal void Build(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (_serviceBuilders.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cached configuration was empty. At least one cache service must be configured.");
            }

            _serviceBuilders.ForEach(builder => builder(services));
        }
    }
}