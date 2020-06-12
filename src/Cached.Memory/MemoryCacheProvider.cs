namespace Cached.Memory
{
    using System;
    using System.Threading.Tasks;
    using Caching;
    using Microsoft.Extensions.Caching.Memory;

    internal sealed class MemoryCacheProvider : IMemory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _options;

        public MemoryCacheProvider(
            IMemoryCache memoryCache,
            MemoryCacheEntryOptions options)
        {
            _options = options;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public Task Set<T>(string key, T item)
        {
            EnsureNotDisposed();

            _memoryCache.Set(key, item, _options);
            return Task.CompletedTask;
        }

        public Task<CachedValueResult<T>> TryGet<T>(string key)
        {
            EnsureNotDisposed();

            if (_memoryCache.TryGetValue(key, out object dataFromCache) && dataFromCache is T castItem)
            {
                return Task.FromResult(CachedValueResult<T>.Hit(castItem));
            }

            return Task.FromResult(CachedValueResult<T>.Miss);
        }

        public Task Remove(string key)
        {
            EnsureNotDisposed();

            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        private bool _disposed;

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(_memoryCache));
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _memoryCache.Dispose();
                _disposed = true;
            }
        }
    }
}