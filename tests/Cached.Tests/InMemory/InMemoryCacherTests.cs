namespace Cached.Tests.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.InMemory;
    using Cached.Locking;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Primitives;
    using Moq;
    using Xunit;

    public class InMemoryCacherTests : CacherTestsBase
    {
        private readonly Mock<IMemoryCache> _memoryCacheMock;

        public InMemoryCacherTests()
        {
            _memoryCacheMock = new Mock<IMemoryCache>();
            _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns(TryGetFakeCacheValue);

            _memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<string>()))
                .Returns((string key) => new FakeCacheEntry(key, SetFakeCache));
        }

        protected override ICacher GetCacher(ILock cacheLock)
        {
            return new InMemoryCacher(cacheLock, _memoryCacheMock.Object, null);
        }

        public class Throws
        {
            [Fact]
            public void If_MemoryCache_Argument_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(
                    ()=> InMemoryCacher.New(
                        null, 
                        new MemoryCacheEntryOptions()));
            }
        }

        public class NewMethod
        {
            [Fact]
            public void Returns_Valid_Instance()
            {
                // Arrange
                var cache = new MemoryCache(new MemoryCacheOptions());

                // Act
                IInMemoryCacher instance = InMemoryCacher.New(cache);

                // Assert
                Assert.NotNull(instance);

                // Teardown
                cache.Dispose();
            }
        }

        public class DefaultMethod
        {
            [Fact]
            public void Returns_Valid_Instance()
            {
                // Arrange, Act
                IInMemoryCacher instance = InMemoryCacher.Default();

                // Assert
                Assert.NotNull(instance);
            }

            [Fact]
            public async Task Generates_Instances_That_Shares_MemoryCache()
            {
                // Arrange
                IInMemoryCacher instance1 = InMemoryCacher.Default(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) });
                IInMemoryCacher instance2 = InMemoryCacher.Default();

                // Act
                var value1 = await instance1.GetOrFetchAsync("default_share_instance_key", key => Task.FromResult("abc"));
                var value2 = await instance2.GetOrFetchAsync("default_share_instance_key", key => Task.FromResult("cde"));

                // Assert
                Assert.Equal("abc", value1);
                Assert.Equal("abc", value2);
            }
        }
    }

    internal class FakeCacheEntry : ICacheEntry
    {
        private readonly Action<string, object> _valueStore;

        public FakeCacheEntry(string key, Action<string, object> valueStore)
        {
            Key = key;
            _valueStore = valueStore;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public object Key { get; }

        public object Value
        {
            get => default;
            set => _valueStore((string)Key, value);
        }

        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public IList<IChangeToken> ExpirationTokens { get; }
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; }
        public CacheItemPriority Priority { get; set; }
        public long? Size { get; set; }
    }
}
