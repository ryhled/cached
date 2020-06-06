namespace Cached.Configuration
{
    /// <summary>
    ///     Implementation-abstracted dependency resolver.
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        ///     Resolves a specific type, based on the underlying IOC implementation that is configured.
        /// </summary>
        /// <typeparam name="T">The type to resolve an instance of.</typeparam>
        /// <returns>An instance of the required type.</returns>
        T GetService<T>();
    }
}