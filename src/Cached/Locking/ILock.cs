namespace Cached.Locking
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     Asynchronous lock that lock based on key object.
    /// </summary>
    public interface ILock
    {
        /// <summary>
        ///     Locks a key object asynchronously.
        /// </summary>
        /// <param name="key">The key object to lock by.</param>
        /// <returns>A lock disposer, responsible for correctly unlocking and de-registering the lock entry.</returns>
        Task<IDisposable> LockAsync(object key);
    }
}