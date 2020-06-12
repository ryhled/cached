namespace Cached.Configuration
{
    using Caching;

    /// <summary>
    ///     Caching options, used to configure Cached.
    /// </summary>
    public interface ICachedOptions
    {
        /// <summary>
        ///     Adds a Cached service.
        /// </summary>
        /// <typeparam name="TProvider">The type of cache provider used for the service.</typeparam>
        /// <param name="builder">The service builder containing the service configuration.</param>
        void AddService<TProvider>(ServiceBuilder<TProvider> builder) where TProvider : ICacheProvider;
    }
}