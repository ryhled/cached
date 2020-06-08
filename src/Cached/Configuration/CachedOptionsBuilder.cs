namespace Cached.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Configures cached for net core, and net 5+, integration.
    /// </summary>
    public sealed class CachedOptionsBuilder
    {
        private readonly List<Action<IServiceCollection>> _serviceFactories = new List<Action<IServiceCollection>>();

        internal void AddService(Action<IServiceCollection> serviceFactory)
        {
            _serviceFactories.Add(serviceFactory);
        }

        internal void Build(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (_serviceFactories.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cached configuration was empty. At least one cache service must be configured.");
            }

            _serviceFactories.ForEach(factory => factory(services));
        }
    }
}