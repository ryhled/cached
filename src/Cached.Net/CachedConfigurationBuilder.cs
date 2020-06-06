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

        public void TryAddSingleton<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            _serviceDescriptors.Add(ServiceDescriptor.Singleton<TFrom, TTo>());
        }

        public void TryAddCacher<TCacher>(Func<IResolver, TCacher> cacherFactory)
            where TCacher : class, ICacher
        {
            _serviceDescriptors.Add(ServiceDescriptor.Singleton(ToNetFactory(cacherFactory)));
        }

        public void TryAddCached<TFrom, TTo, TResponse, TParam>(Func<IResolver, TTo> cachedFactory)
            where TFrom : class
            where TTo : class, TFrom, ICached<TResponse, TParam>
        {
            _serviceDescriptors.Add(ServiceDescriptor.Transient<TFrom, TTo>(ToNetFactory(cachedFactory)));
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
            => p
                => factory(new NetResolver(p));
    }
}