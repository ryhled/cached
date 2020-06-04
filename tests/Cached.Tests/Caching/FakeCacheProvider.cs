namespace Cached.Tests.Caching
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Cached.Caching;

    internal class FakeCacheProvider : ICacheProvider
    {
        public int TriedToGet { get; private set; } = 0;
        public int TriedToSet { get; private set; } = 0;
        public IDictionary<string, object> Items = new Dictionary<string, object>();

        private readonly bool _failFirstRead;

        public FakeCacheProvider(bool failFirstRead = false)
        {
            _failFirstRead = failFirstRead;
        }

        public Task<bool> TryGetFromCache<T>(string key, out T item)
        {
            ++TriedToGet;

            if ((!_failFirstRead || TriedToGet > 1) && Items.TryGetValue(key, out object fromCache))
            {
                item = (T) fromCache;
                return Task.FromResult(true);
            }

            item = default;
            return Task.FromResult(false);
        }

        public Task WriteToCache<T>(string key, T item)
        {
            ++TriedToSet;
            Items.Add(new KeyValuePair<string, object>(key, item));
            return Task.FromResult(Task.CompletedTask);
        }
    }
}