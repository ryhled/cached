namespace Cached.Tests.Caching
{
    using System;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.Locking;
    using Moq;
    using Xunit;

    public class CacheHandlerTests
    {
        public class Constructor
        {
            public class Throws
            {
                [Fact]
                public void When_Provider_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new CacheHandler<ICacheProvider>(
                            null, 
                            new Mock<ILock>().Object));
                }

                [Fact]
                public void When_Lock_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new CacheHandler<ICacheProvider>(
                            new Mock<ICacheProvider>().Object,
                            null));
                }
            }
        }

        public class GetOrFetchAsyncMethod
        {
            public class Throws
            {
                 // TODO: Throw on null key?

                [Fact]
                public async Task When_FetchFactory_Argument_Is_Null()
                {
                    // Arrange
                    var handler = new CacheHandler<ICacheProvider>(
                        new Mock<ICacheProvider>().Object,
                        new Mock<ILock>().Object);

                    // Act, Assert
                    await Assert.ThrowsAsync<ArgumentNullException>(
                        async () => await handler.GetOrFetchAsync<string>("_", null));
                }
            }

            [Fact]
            public async Task Get_From_Fetch_Function_If_Cache_Miss()
            {
                // Arrange
                var providerMock = new Mock<ICacheProvider>();
                providerMock.Setup(m => m.TryGet<string>("key"))
                    .Returns(Task.FromResult(CachedValueResult<string>.Miss));
                var lockMock = new Mock<ILock>();
                var handler = new CacheHandler<ICacheProvider>(providerMock.Object, lockMock.Object);

                // Act
                var result = await handler.GetOrFetchAsync(
                    "key",
                    key => Task.FromResult(key + "_fetched"));

                // Assert
                Assert.Equal("key_fetched", result);
                Assert.Equal(handler.Provider, providerMock.Object);
                lockMock.Verify(l => l.LockAsync(It.IsAny<object>()), Times.Once);
                lockMock.Verify(l => l.LockAsync("key"), Times.Once);
                providerMock.Verify(m => m.TryGet<It.IsAnyType>(It.IsAny<string>()), Times.Exactly(2));
                providerMock.Verify(m => m.TryGet<string>("key"), Times.Exactly(2));
                providerMock.Verify(m => m.Set(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
                providerMock.Verify(m => m.Set("key", "key_fetched"), Times.Once);
            }

            [Fact]
            public async Task Get_From_Cache_If_Cache_Miss_But_Queued_Late()
            {
                // Arrange
                var hit = false;
                var providerMock = new Mock<ICacheProvider>();
                providerMock.Setup(m => m.TryGet<string>("key"))
                    .Returns((string _) =>
                    {
                        if (hit)
                        {
                            return Task.FromResult(CachedValueResult<string>.Hit("cached_value"));
                        }
                        hit = true;
                        return Task.FromResult(CachedValueResult<string>.Miss);
                    });

                var lockMock = new Mock<ILock>();
                var handler = new CacheHandler<ICacheProvider>(providerMock.Object, lockMock.Object);

                // Act
                var result = await handler.GetOrFetchAsync(
                    "key",
                    key => Task.FromResult(key + "_fetched"));

                // Assert
                Assert.Equal("cached_value", result);
                lockMock.Verify(l => l.LockAsync(It.IsAny<object>()), Times.Once);
                lockMock.Verify(l => l.LockAsync("key"), Times.Once);
                providerMock.Verify(m => m.Set(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
                providerMock.Verify(m => m.TryGet<It.IsAnyType>(It.IsAny<string>()), Times.Exactly(2));
                providerMock.Verify(m => m.TryGet<string>("key"), Times.Exactly(2));
            }

            [Fact]
            public async Task Gets_Cached_Item_When_Exist()
            {
                // Arrange
                var providerMock = new Mock<ICacheProvider>();
                providerMock.Setup(m => m.TryGet<string>("key"))
                    .Returns(Task.FromResult(CachedValueResult<string>.Hit("cached_value")));
                var lockMock = new Mock<ILock>();
                var handler = new CacheHandler<ICacheProvider>(providerMock.Object, lockMock.Object);

                // Act
                var result = await handler.GetOrFetchAsync(
                    "key",
                    key => Task.FromResult(key + "_fetched"));

                // Assert
                Assert.Equal("cached_value", result);
                lockMock.Verify(l => l.LockAsync(It.IsAny<object>()), Times.Never);
                providerMock.Verify(m => m.TryGet<It.IsAnyType>(It.IsAny<string>()), Times.Once);
                providerMock.Verify(m => m.TryGet<string>("key"), Times.Once);
                providerMock.Verify(m => m.Set(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
            }
        }
    }
}