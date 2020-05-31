namespace Cached.Tests.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Cached.Locking;
    using Configuration;
    using Locking;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    public sealed class InMemoryCacherTests
    {
        private static string GetInternalKey(string key, Type valueType)
        {
            return $"{nameof(InMemoryCacher)}_{valueType.FullName}_{key}";
        }

        private delegate bool TryGetValueReturns(string key, out object cachedData);

        public sealed class Constructor
        {
            private readonly Func<DateTimeOffset> _fakeNowFactory = () => DateTimeOffset.ParseExact(
                "02/11/2020 09.21.00 +01:00",
                "dd/MM/yyyy HH.mm.ss zzz", CultureInfo.InvariantCulture);

            [Fact]
            public async Task Will_rely_on_memory_cache_config_if_no_settings_provided()
            {
                // Arrange
                var lockMock = new Mock<ILock>();
                var memoryCacheMock = new Mock<IMemoryCache>();
                ICacheEntry entryResult = default;
                memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<string>()))
                    .Returns((string key) =>
                    {
                        entryResult = new FakeMemoryCacheEntry(key);
                        return entryResult;
                    });
                var cacher = new InMemoryCacher(memoryCacheMock.Object, lockMock.Object, null, _fakeNowFactory);

                // Act
                await cacher.GetOrFetchAsync("", () => Task.FromResult(""));

                // Assert
                // TODO: Assert that no memory cache options are set for cache entry.
            }

            [Fact]
            public async Task Will_use_provided_settings_when_specified()
            {
                // Arrange
                var lockMock = new Mock<ILock>();
                var memoryCacheMock = new Mock<IMemoryCache>();
                ICacheEntry entryResult = default;
                memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<string>()))
                    .Returns((string key) =>
                    {
                        entryResult = new FakeMemoryCacheEntry(key);
                        return entryResult;
                    });
                var settings = new CachedSettings(TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(160));
                var cacher = new InMemoryCacher(memoryCacheMock.Object, lockMock.Object, settings, _fakeNowFactory);

                // Act
                await cacher.GetOrFetchAsync("", () => Task.FromResult(""));

                // Assert
                Assert.Equal(_fakeNowFactory().Add(settings.AbsoluteExpiration), entryResult.AbsoluteExpiration.Value);
                Assert.Equal(settings.SlidingExpiration, entryResult.SlidingExpiration.Value);
            }
        }

        public sealed class NewMethod
        {
            public sealed class Throws
            {
                [Fact]
                public void If_Cache_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => InMemoryCacher.New(null, new CachedSettings()));
                }
            }

            public sealed class CreatesInstance
            {
                [Fact]
                public void When_Cache_Argument_And_Settings_Are_Set()
                {
                    // Arrange, Act
                    var result = InMemoryCacher.New(new Mock<IMemoryCache>().Object, new CachedSettings());

                    // Assert
                    Assert.NotNull(result);
                }

                [Fact]
                public void When_Cache_Argument_Is_Set()
                {
                    // Arrange, Act
                    var result = InMemoryCacher.New(new Mock<IMemoryCache>().Object);

                    // Assert
                    Assert.NotNull(result);
                }
            }
        }

        public sealed class GetOrFetchAsyncMethod
        {
            public GetOrFetchAsyncMethod()
            {
                _fakeCache = new ConcurrentDictionary<string, FakeMemoryCacheEntry>();
                _cacherLockMock = new Mock<ILock>();
                _memoryCacheMock = new Mock<IMemoryCache>();

                _cacherLockMock.Setup(c => c.LockAsync(It.IsAny<object>()))
                    .Returns(Task.FromResult<IDisposable>(new FakeDisposable()));

                _memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<string>())).Returns((string key) =>
                {
                    var entry = new FakeMemoryCacheEntry(key);
                    _fakeCache[key] = entry;
                    return entry;
                });

                _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny)).Returns(
                    new TryGetValueReturns((string key, out object cachedData) =>
                    {
                        var found = _fakeCache.TryGetValue(key, out FakeMemoryCacheEntry cached);
                        cachedData = cached?.Value;
                        return found;
                    }));
            }

            private readonly ConcurrentDictionary<string, FakeMemoryCacheEntry> _fakeCache;
            private readonly Mock<IMemoryCache> _memoryCacheMock;
            private readonly Mock<ILock> _cacherLockMock;
            private readonly Func<DateTimeOffset> _fakeNowFactory = () => DateTimeOffset.UtcNow;

            private InMemoryCacher NewInMemoryCacher()
            {
                return new InMemoryCacher(_memoryCacheMock.Object, _cacherLockMock.Object, new CachedSettings(),
                    _fakeNowFactory);
            }

            [Fact]
            public async Task Adds_missing_data_to_cache_using_the_fetch_function()
            {
                // Arrange
                InMemoryCacher memoryCacher = NewInMemoryCacher();
                const string ageKey = "age";

                // Act
                var value = await memoryCacher.GetOrFetchAsync(ageKey, () => Task.FromResult(23));

                // Assert
                Assert.Equal(23, value);
                Assert.Equal(23, _fakeCache[GetInternalKey(ageKey, typeof(int))].Value);
                _cacherLockMock.Verify(c => c.LockAsync(It.IsAny<object>()), Times.Once);
                _cacherLockMock.Verify(c => c.LockAsync(GetInternalKey(ageKey, typeof(int))), Times.Once);
                _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny),
                    Times.Exactly(2));
            }

            [Fact]
            public async Task Gets_cached_data_without_fetching_data_from_function()
            {
                // Arrange
                const string nameKey = "name";
                var internalName = GetInternalKey(nameKey, typeof(string));
                InMemoryCacher memoryCacher = NewInMemoryCacher();
                _fakeCache[internalName] = new FakeMemoryCacheEntry(internalName) {Value = "John"};

                // Act
                var value = await memoryCacher.GetOrFetchAsync(nameKey, () => Task.FromResult("George"));

                // Assert
                Assert.Equal("John", value);
                _cacherLockMock.Verify(c => c.LockAsync(It.IsAny<object>()), Times.Never);
                _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny), Times.Once);
            }
        }
    }
}