namespace Cached
{
    using System;
    using Caching;

    /// <summary>
    ///     Configures cached for net core, and net 5+, integration.
    /// </summary>
    public interface ICachedConfigurationBuilder
    {
        /// <summary>
        ///     Adds a cached service in singleton scope.
        /// </summary>
        /// <typeparam name="TFrom">The type of object to be used for injection.</typeparam>
        /// <typeparam name="TTo">The type of the concrete object meant to fill the injection.</typeparam>
        /// <param name="serviceFactory">The ServiceFactory that ultimately creates the instance.</param>
        void AddSingletonService<TFrom, TTo>(Func<IServiceProvider, TTo> serviceFactory)
            where TFrom : class
            where TTo : class, TFrom, ICachedService;

        /// <summary>
        ///     Adds a cached service in transient scope.
        /// </summary>
        /// <typeparam name="TFrom">The type of object to be used for injection.</typeparam>
        /// <typeparam name="TTo">The type of the concrete object meant to fill the injection.</typeparam>
        /// <param name="serviceFactory">The ServiceFactory that ultimately creates the instance.</param>
        void AddTransientService<TFrom, TTo>(Func<IServiceProvider, TTo> serviceFactory)
            where TFrom : class
            where TTo : class, TFrom, ICachedService;
    }
}