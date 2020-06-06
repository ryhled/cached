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
        ///     Adds Cacher service (in singleton scope).
        /// </summary>
        /// <typeparam name="TCacher">The interface, from which to inject the cacher service.</typeparam>
        /// <param name="cacherFactory">The ServiceFactory that ultimately creates the instance.</param>
        void AddCacher<TCacher>(Func<IServiceProvider, TCacher> cacherFactory) 
            where TCacher : class, ICacher;

        /// <summary>
        ///     Adds a cached service (in transient scope).
        /// </summary>
        /// <typeparam name="TFrom">The type of object to be used for injection.</typeparam>
        /// <typeparam name="TTo">The type of the concrete object meant to fill the injection.</typeparam>
        /// /// <typeparam name="TResponse">The type of object that the cached requests will respond with.</typeparam>
        /// <typeparam name="TParam">The type object used as cached request argument.</typeparam>
        /// <param name="cachedFactory">The ServiceFactory that ultimately creates the instance.</param>
        void AddCached<TFrom, TTo, TResponse, TParam>(Func<IServiceProvider, TTo> cachedFactory)
            where TFrom : class
            where TTo : class, TFrom, ICached<TResponse, TParam>;
    }
}