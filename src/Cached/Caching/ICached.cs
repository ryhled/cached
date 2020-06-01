namespace Cached.Caching
{
    using System.Threading.Tasks;

    /// <summary>
    ///     Abstracts a lazily cached service call.
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected.</typeparam>
    /// <typeparam name="TParam">The type of parameter that is needed.</typeparam>
    public interface ICached<TResponse, in TParam>
    {
        /// <summary>
        ///     Tries to get data from cached based on provided argument.
        ///     If it does not exist in cache it is fetched from predefined factory function.
        /// </summary>
        /// <param name="arg">The argument that will be used for fetching the data.</param>
        /// <returns>The data fetched based on the provided argument.</returns>
        Task<TResponse> GetOrFetchAsync(TParam arg);
    }
}