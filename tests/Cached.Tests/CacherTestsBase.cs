namespace Cached.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.Locking;
    using Moq;
    using Xunit;

    public abstract class CacherTestsBase
    {
        private readonly IDictionary<string, object> _cacheItems = new Dictionary<string, object>();
        protected abstract ICacher GetCacher(ILock cacheLock);

        protected delegate bool TryGetValueReturns(string key, out object item);

        private bool _forceCacheMiss;

        protected void SetFakeCache(string key, object item)
            => _cacheItems.Add(new KeyValuePair<string, object>(key, item));

        protected TryGetValueReturns TryGetFakeCacheValue => (string key, out object item) =>
        {
            if (_cacheItems.TryGetValue(key, out object fromCache) && !_forceCacheMiss)
            {
                item = fromCache;
                return true;
            }

            _forceCacheMiss = false;
            item = default;
            return false;
        };

        [Fact]
        public void Throws_If_Lock_Argument_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => GetCacher(null));
        }

        [Fact]
        public async Task GetOrFetchAsync_Throws_If_FetchFactory_Argument_Is_Null()
        {
            // Arrange
            ICacher cacher = GetCacher(new Mock<ILock>().Object);

            // Act, Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await cacher.GetOrFetchAsync("", (Func<string, Task<string>>)null));
        }

        [Fact]
        public async Task GetOrFetchAsync_Gets_Cached_Item_When_Exist()
        {
            // Arrange
            SetFakeCache("cached-key", "cached-data");
            var lockMock = new Mock<ILock>();
            ICacher cacher = GetCacher(lockMock.Object);

            // Act
            var result = await cacher.GetOrFetchAsync("cached-key", key => Task.FromResult(key + "-fetched"));

            // Assert
            Assert.Equal("cached-data", result);
            lockMock.Verify(l => l.LockAsync(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task GetOrFetchAsync_Get_From_Cache_If_Miss_Queued_Late()
        {
            // Arrange
            _forceCacheMiss = true;
            SetFakeCache("miss-queued-key", "data-fetched_by_other_request_during_lock_queue");
            var lockMock = new Mock<ILock>();
            ICacher cacher = GetCacher(lockMock.Object);

            // Act
            var result = await cacher.GetOrFetchAsync("miss-queued-key", key => Task.FromResult(key + "-fetched"));

            // Assert
            Assert.Equal("data-fetched_by_other_request_during_lock_queue", result);
            lockMock.Verify(l => l.LockAsync(It.IsAny<object>()), Times.Once);
        }
    }
}
