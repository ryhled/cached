namespace Cached.Configuration
{
    using System;

    /// <summary>
    ///     Configures cached for net core, and net 5+, integration.
    /// </summary>
    public interface ICachedConfigurationBuilder
    {
        /// <summary>
        ///     Adds Cacher service (in singleton scope).
        /// </summary>
        /// <typeparam name="TTo">The type of the concrete object meant to fill the injection.</typeparam>
        /// <param name="serviceFactory">The ServiceFactory that ultimately creates the instance.</param>
        void TryAddSingleton<TTo>(Func<IResolver, TTo> serviceFactory) 
            where TTo : class;

        /// <summary>
        ///     Adds a cached service (in transient scope).
        /// </summary>
        /// <typeparam name="TTo">The type of the concrete object meant to fill the injection.</typeparam>
        /// <param name="serviceFactory">The ServiceFactory that ultimately creates the instance.</param>
        void TryAddTransient<TTo>(Func<IResolver, TTo> serviceFactory)
            where TTo : class;

        /// <summary>
        /// Tries to add a singleton-scoped service.
        /// </summary>
        /// <typeparam name="TFrom">The public type to inject from.</typeparam>
        /// <typeparam name="TTo">The concrete type that will fill the injection.</typeparam>
        void TryAddSingleton<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom;
    }
}