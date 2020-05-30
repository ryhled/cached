namespace Cached.Net
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    internal sealed class CachedOptionsBuilder : ICachedOptions
    {
        private readonly List<ServiceDescriptor> _serviceDescriptors
            = new List<ServiceDescriptor>();

        public CachedSettings GlobalSettings { get; set; }

        public void AddSingletonService<TFrom, TTo>(Func<IServiceProvider, TTo> serviceFactory)
            where TFrom : class
            where TTo : class, TFrom, ICachedService
        {
            _serviceDescriptors.Add(ServiceDescriptor.Singleton<TFrom, TTo>(provider => serviceFactory(provider)));
        }

        public void AddTransientService<TFrom, TTo>(Func<IServiceProvider, TTo> serviceFactory)
            where TFrom : class
            where TTo : class, TFrom, ICachedService
        {
            _serviceDescriptors.Add(ServiceDescriptor.Transient<TFrom, TTo>(provider => serviceFactory(provider)));
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

            CachedSettings globalSettings = GlobalSettings ?? new CachedSettings();

            _serviceDescriptors.ForEach(d => services.TryAdd(d));
        }
    }
}