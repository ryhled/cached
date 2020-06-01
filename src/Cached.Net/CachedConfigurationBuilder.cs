namespace Cached.Net
{
    using System;
    using System.Collections.Generic;
    using Caching;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    internal sealed class CachedConfigurationBuilder : ICachedConfigurationBuilder
    {
        private readonly List<ServiceDescriptor> _serviceDescriptors
            = new List<ServiceDescriptor>();

        public void AddSingletonService<TFrom, TTo>(Func<IServiceProvider, TTo> serviceFactory)
            where TFrom : class
            where TTo : class, TFrom, ICachedService
        {
            _serviceDescriptors.Add(ServiceDescriptor.Singleton<TFrom, TTo>(serviceFactory));
        }

        public void AddTransientService<TFrom, TTo>(Func<IServiceProvider, TTo> serviceFactory)
            where TFrom : class
            where TTo : class, TFrom, ICachedService
        {
            _serviceDescriptors.Add(ServiceDescriptor.Transient<TFrom, TTo>(serviceFactory));
        }

        internal void Build(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (_serviceDescriptors.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cached configuration was empty. At least one cache option must be set.");
            }

            _serviceDescriptors.ForEach(services.TryAdd);
        }
    }
}