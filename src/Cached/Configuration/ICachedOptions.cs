namespace Cached.Configuration
{
    using System;

    /// <summary>
    ///     Configures cached for net core, and net 5+, integration.
    /// </summary>
    public interface ICachedOptions
    {
        /// <summary>
        ///     Specifiy global settings to be used for all Cached services.
        ///     (Service specific settings override global settings).
        /// </summary>
        CachedSettings GlobalSettings { get; set; }

        /// <summary>
        ///     Adds a cached service in singleton scope.
        /// </summary>
        /// <typeparam name="TFrom">The type of object to be used for injection.</typeparam>
        /// <typeparam name="TTo">The type of the concrete object meant to fill the injection.</typeparam>
        /// <param name="serviceFactory">The servicefactory that ultimately creates the instance.</param>
        void AddSingletonService<TFrom, TTo>(Func<IServiceProvider, TTo> serviceFactory)
            where TFrom : class
            where TTo : class, TFrom, ICachedService;

        /// <summary>
        ///     Adds a cached service in transient scope.
        /// </summary>
        /// <typeparam name="TFrom">The type of object to be used for injection.</typeparam>
        /// <typeparam name="TTo">The type of the concrete object meant to fill the injection.</typeparam>
        /// <param name="serviceFactory">The servicefactory that ultimately creates the instance.</param>
        void AddTransientService<TFrom, TTo>(Func<IServiceProvider, TTo> serviceFactory)
            where TFrom : class
            where TTo : class, TFrom, ICachedService;
    }
}