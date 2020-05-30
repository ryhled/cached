namespace Cached.Tests.InMemory
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Primitives;

    internal class FakeMemoryCacheEntry : ICacheEntry
    {
        public FakeMemoryCacheEntry(object key)
        {
            Key = key;
        }

        public object Key { get; }

        public object Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }

        public IList<IChangeToken> ExpirationTokens { get; set; }

        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; set; }

        public CacheItemPriority Priority { get; set; }
        public long? Size { get; set; }

        public void Dispose()
        {
        }
    }
}