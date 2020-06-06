namespace Cached.Net
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    internal sealed class CachedConfigurationBuilder : ICachedConfigurationBuilder
    {
        private readonly List<ServiceDescriptor> _serviceDescriptors
            = new List<ServiceDescriptor>();

        public void TryAddSingleton<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            _serviceDescriptors.Add(ServiceDescriptor.Singleton<TFrom, TTo>());
        }

        public void TryAddSingleton<TTo>(Func<IResolver, TTo> cacherFactory)
            where TTo : class
        {
            _serviceDescriptors.Add(ServiceDescriptor.Singleton(ToNetFactory(cacherFactory)));
        }

        public void TryAddTransient<TTo>(Func<IResolver, TTo> cacherFactory)
            where TTo : class
        {
            _serviceDescriptors.Add(ServiceDescriptor.Transient(ToNetFactory(cacherFactory)));
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

        private static Func<IServiceProvider, TTo> ToNetFactory<TTo>(Func<IResolver, TTo> factory)
        {
            return p
                => factory(new NetResolver(p));
        }
    }
}