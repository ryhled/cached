namespace Cached.Tests.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Cached.Locking;
    using Caching;
    using Locking;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    public sealed class InMemoryCacherTests
    {
        private static string GetInternalKey(string key, Type valueType)
        {
            return $"{nameof(Cacher)}|{valueType.FullName}|{key}";
        }

        private delegate bool TryGetValueReturns(string key, out object cachedData);

        public sealed class Constructor
        {
            [Fact]
            public async Task Relies_On_MemoryCache_Config_If_No_Settings_Are_Provided()
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
                var cacher = new InMemoryCacher(memoryCacheMock.Object, lockMock.Object, null);

                // Act
                await cacher.GetOrFetchAsync("", _ => Task.FromResult(""));

                // Assert
                // TODO: Assert that no memory cache options are set for cache entry.
            }

            [Fact]
            public async Task Will_Use_Provided_Options_When_Specified()
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
                var options = new MemoryCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)};
                var cacher = new InMemoryCacher(memoryCacheMock.Object, lockMock.Object, options);

                // Act
                await cacher.GetOrFetchAsync("", _ => Task.FromResult(""));

                // Assert
                Assert.Equal(options.AbsoluteExpirationRelativeToNow, entryResult.AbsoluteExpirationRelativeToNow);
            }
        }

        public sealed class DefaultMethod
        {
            public sealed class CreatesInstance
            {
                [Fact]
                public void With_Empty_Constructor()
                {
                    //Arrange, Act
                    InMemoryCacher result = InMemoryCacher.Default();

                    //Assert
                    Assert.NotNull(result);
                }

                [Fact]
                public void With_Settings_Argument_Constructor()
                {
                    //Arrange, Act
                    InMemoryCacher result = InMemoryCacher.Default(new MemoryCacheEntryOptions());

                    //Assert
                    Assert.NotNull(result);
                }
            }
        }

        public sealed class NewMethod
        {
            public sealed class CreatesInstance
            {
                [Fact]
                public void When_Cache_Argument_And_Settings_Are_Set()
                {
                    // Arrange, Act
                    var result = InMemoryCacher.New(new Mock<IMemoryCache>().Object, new MemoryCacheEntryOptions());

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

            private InMemoryCacher NewInMemoryCacher()
            {
                return new InMemoryCacher(_memoryCacheMock.Object, _cacherLockMock.Object,
                    new MemoryCacheEntryOptions());
            }

            [Fact]
            public async Task Will_Pass_The_Same_Key_That_Was_Provided()
            {
                // Arrange
                InMemoryCacher memoryCacher = NewInMemoryCacher();

                // Act
                var result = await memoryCacher.GetOrFetchAsync("abc123", Task.FromResult);

                // Assert
                Assert.Equal("abc123", result);
            }

            [Fact]
            public async Task Adds_Missing_Data_To_Cache_Using_The_Fetch_Factory()
            {
                // Arrange
                InMemoryCacher memoryCacher = NewInMemoryCacher();

                // Act
                var value = await memoryCacher.GetOrFetchAsync("age-key", _ => Task.FromResult(23));

                // Assert
                Assert.Equal(23, value);
                Assert.Equal(23, _fakeCache[GetInternalKey("age-key", typeof(int))].Value);
                _cacherLockMock.Verify(c => c.LockAsync(It.IsAny<object>()), Times.Once);
                _cacherLockMock.Verify(c => c.LockAsync(GetInternalKey("age-key", typeof(int))), Times.Once);
                _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny),
                    Times.Exactly(2));
            }

            [Fact]
            public async Task Gets_Cached_Data_Without_Using_Fetch_Factory()
            {
                // Arrange
                var internalName = GetInternalKey("some-name-key", typeof(string));
                InMemoryCacher memoryCacher = NewInMemoryCacher();
                _fakeCache[internalName] = new FakeMemoryCacheEntry(internalName) {Value = "John"};

                // Act
                var value = await memoryCacher.GetOrFetchAsync("some-name-key", _ => Task.FromResult("George"));

                // Assert
                Assert.Equal("John", value);
                _cacherLockMock.Verify(c => c.LockAsync(It.IsAny<object>()), Times.Never);
                _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny), Times.Once);
            }

            [Fact]
            public async Task Do_Not_Hit_Fetch_Even_If_Cache_Missed_In_Case_Other_Request_Ran_Fetch_First()
            {
                // Arrange
                var fetchCounter = 0;
                var hasHitOnce = false;

                bool TryGetValueFakeCallback(string key, out object cachedData)
                {
                    cachedData = "John";
                    if (hasHitOnce)
                    {
                        return key.EndsWith("some-name-key");
                    }

                    hasHitOnce = true;
                    return false;
                }

                Task<string> FakeFetchFactory(string key)
                {
                    fetchCounter++;
                    return Task.FromResult(string.Empty);
                }

                _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                    .Returns(new TryGetValueReturns(TryGetValueFakeCallback));

                // Act
                var value = await NewInMemoryCacher().GetOrFetchAsync("some-name-key", FakeFetchFactory);

                // Assert
                Assert.Equal("John", value);
                Assert.Equal(0, fetchCounter);
                _cacherLockMock.Verify(c => c.LockAsync(It.IsAny<object>()), Times.Once);
                _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny), Times.Exactly(2));
            }
        }
    }
}