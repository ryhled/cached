namespace Cached.Tests.Caching
{
    using System;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.Locking;
    using Moq;
    using Xunit;

    public sealed class CacherTests
    {
        public static string GetInternalKey<TArgType>(string key)
            => $"{nameof(Cacher<FakeCacheProvider>)}|{typeof(TArgType).FullName}|{key}";

        public sealed class Constructor
        {
            public sealed class Throws
            {
                [Fact]
                public void If_Lock_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(()
                        => new Cacher<FakeCacheProvider>(default, new FakeCacheProvider()));
                }

                [Fact]
                public void If_Provider_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(()
                        => new Cacher<FakeCacheProvider>(new Mock<ILock>().Object, default));
                }
            }
        }

        public sealed class GetOrFetchAsyncMethod
        {
            public sealed class Throws
            {
                [Fact]
                public async Task If_FetchFactory_Argument_Is_Null()
                {
                    // Arrange
                    var cacher = new Cacher<FakeCacheProvider>(new Mock<ILock>().Object, new FakeCacheProvider());

                    // Act, Assert
                    await Assert.ThrowsAsync<ArgumentNullException>(
                        async () => await cacher.GetOrFetchAsync("", (Func<string, Task<string>>)null));
                }
            }

            public sealed class FetchValueFromSource
            {
                [Fact]
                public async Task Fetch_Item_From_Source_When_Not_Exist_In_Cache()
                {
                    // Arrange
                    var fakeProvider = new FakeCacheProvider();
                    var lockMock = new Mock<ILock>();
                    var cacher = new Cacher<FakeCacheProvider>(lockMock.Object, fakeProvider);

                    // Act
                    var result = await cacher.GetOrFetchAsync("some-key", key => Task.FromResult(key + "-fetched"));

                    // Assert
                    Assert.Equal("some-key-fetched", result);
                    Assert.Equal(2, fakeProvider.TriedToGet);
                    Assert.Equal(1, fakeProvider.TriedToSet);
                    lockMock.Verify(l => l.LockAsync(It.IsAny<object>()), Times.Once);

                }
            }

            public sealed class GetsValueFromCache
            {
                [Fact]
                public async Task Get_Item_From_Cache_When_It_Exist()
                {
                    // Arrange
                    var fakeProvider = new FakeCacheProvider();
                    fakeProvider.Items.Add(GetInternalKey<string>("some-key"), "cached-data");
                    var lockMock = new Mock<ILock>();
                    var cacher = new Cacher<FakeCacheProvider>(lockMock.Object, fakeProvider);

                    // Act
                    var result = await cacher.GetOrFetchAsync("some-key", key => Task.FromResult(key + "-fetched"));

                    // Assert
                    Assert.Equal("cached-data", result);
                    Assert.Equal(1, fakeProvider.TriedToGet);
                    Assert.Equal(0, fakeProvider.TriedToSet);
                    lockMock.Verify(l => l.LockAsync(It.IsAny<object>()), Times.Never);
                }

                [Fact]
                public async Task If_Item_Is_Missing_But_Other_Request_Fetched_First_Then_Cached_Item_Is_returned()
                {
                    // Arrange
                    var fakeProvider = new FakeCacheProvider(true);
                    fakeProvider.Items.Add(GetInternalKey<string>("some-key"), "data-fetched_by_other_request_during_lock_queue");
                    var lockMock = new Mock<ILock>();
                    var cacher = new Cacher<FakeCacheProvider>(lockMock.Object, fakeProvider);

                    // Act
                    var result = await cacher.GetOrFetchAsync("some-key", key => Task.FromResult(key + "-fetched"));

                    // Assert
                    Assert.Equal("data-fetched_by_other_request_during_lock_queue", result);
                    Assert.Equal(2, fakeProvider.TriedToGet);
                    Assert.Equal(0, fakeProvider.TriedToSet);
                    lockMock.Verify(l => l.LockAsync(It.IsAny<object>()), Times.Once);
                }
            }
        }
    }
}
