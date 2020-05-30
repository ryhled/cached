namespace Cached.Locking
{
    using System;
    using System.Threading.Tasks;

    internal interface ILock
    {
        Task<IDisposable> LockAsync(object key);
    }
}