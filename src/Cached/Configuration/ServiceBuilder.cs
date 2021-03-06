﻿namespace Cached.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Caching;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Builds the configuration for cached services.
    /// </summary>
    /// <typeparam name="TProvider">The type of cache provider used for the service.</typeparam>
    public abstract class ServiceBuilder<TProvider> where TProvider : ICacheProvider
    {
        /// <summary>
        ///     Contains the user-provides service descriptors for a specific cache provider.
        /// </summary>
        protected readonly List<ServiceDescriptor> Descriptors = new List<ServiceDescriptor>();

        internal ServiceBuilder()
        {
        }

        internal abstract Action<IServiceCollection> GetBuild();

        /// <summary>
        ///     Adds a new cached function based on the provided types and functions.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response object returned by the fetch factory.</typeparam>
        /// <typeparam name="TArgument">
        ///     The type of the argument object, used for building the cache key and provided to fetch
        ///     factory.
        /// </typeparam>
        /// <param name="keyFactory">The function responsible for turning the argument into a string-based cache key.</param>
        /// <param name="fetchFactory">The function responsible for retrieving the data if not existing in cache.</param>
        public void AddFunction<TResponse, TArgument>(
            Func<TArgument, string> keyFactory,
            Func<IServiceProvider, string, TArgument, Task<TResponse>> fetchFactory)
        {
            if (keyFactory == null)
            {
                throw new ArgumentNullException(nameof(keyFactory));
            }

            if (fetchFactory == null)
            {
                throw new ArgumentNullException(nameof(fetchFactory));
            }

            ServiceDescriptor descriptor = ServiceDescriptor.Transient<ICached<TResponse, TArgument>>(provider =>
                new Cached<TResponse, TArgument, TProvider>(
                    provider.GetService<ICache<TProvider>>(),
                    keyFactory,
                    (key, arg) => fetchFactory.Invoke(provider, key, arg)));

            Descriptors.Add(descriptor);
        }
    }
}