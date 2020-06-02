namespace Cached.Locking
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class KeyBasedLock : ILock
    {
        private static readonly Dictionary<object, Reservable<SemaphoreSlim>> Reserved
            = new Dictionary<object, Reservable<SemaphoreSlim>>();

        public async Task<IDisposable> LockAsync(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await GetOrCreate(key).WaitAsync().ConfigureAwait(false);
            return new Disposer(key);
        }

        private static SemaphoreSlim GetOrCreate(object key)
        {
            lock (Reserved)
            {
                if (!Reserved.TryGetValue(key, out var semaphore))
                {
                    semaphore = new Reservable<SemaphoreSlim>(new SemaphoreSlim(1, 1));
                    Reserved[key] = semaphore;
                }

                semaphore.Reserve();
                return semaphore.Value;
            }
        }

        private sealed class Disposer : IDisposable
        {
            private readonly object _key;

            public Disposer(object key)
            {
                _key = key;
            }

            public void Dispose()
            {
                lock (Reserved)
                {
                    if (!Reserved.TryGetValue(_key, out var item))
                    {
                        throw new InvalidOperationException(
                            $"Reserved item with key '{_key}' not found in dictionary.");
                    }

                    if (item.TryRelease())
                    {
                        Reserved.Remove(_key);
                    }

                    item.Value.Release();
                }
            }
        }
    }
}