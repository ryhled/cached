namespace Cached.Tests.Memory
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using MemoryCache;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    public class MemoryCacheProviderTests
    {
        public class Constructor
        {
            public class Throws
            {
                [Fact]
                public void If_MemoryCache_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new MemoryCacheProvider(null, new MemoryCacheEntryOptions()));
                }
            }
        }

        public class SetMethod
        {
            public class Throws
            {
                [Fact]
                public async Task If_MemoryCache_Is_Disposed()
                {
                    // Arrange
                    var cacheMock = new Mock<IMemoryCache>();
                    var provider = new MemoryCacheProvider(cacheMock.Object, null);

                    // Act
                    provider.Dispose();
                    provider.Dispose(); // Should hit cache dispose only once

                    // Assert
                    await Assert.ThrowsAsync<ObjectDisposedException>(
                        async () => await provider.Set("key1", "value1"));
                    cacheMock.Verify(m => m.Dispose(), Times.Once);
                }
            }

            [Fact]
            public void Sets_Provided_Configuration_Using_Underlying_Provider()
            {
                // Arrange
                DateTimeOffset absoluteExpiration = DateTimeOffset.ParseExact(
                    "2022-01-02T23:01:22-11:22",
                    "yyyy-MM-dd'T'HH:mm:ss.FFFK",
                    CultureInfo.InvariantCulture);

                var cacheMock = new Mock<IMemoryCache>();
                var config = new MemoryCacheEntryOptions {AbsoluteExpiration = absoluteExpiration};
                FakeCacheEntry insertedEntry = null;
                cacheMock.Setup(m => m.CreateEntry(It.IsAny<object>()))
                    .Returns((string key) =>
                    {
                        insertedEntry = new FakeCacheEntry();
                        return insertedEntry;
                    });
                var provider = new MemoryCacheProvider(cacheMock.Object, config);

                // Act
                provider.Set("key1", "value1");

                // Assert
                Assert.Equal(absoluteExpiration, insertedEntry.AbsoluteExpiration);
                cacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
            }

            [Fact]
            public void Sets_Value_Using_Underlying_Provider()
            {
                // Arrange
                var cache = new MemoryCache(new MemoryCacheOptions());
                var provider = new MemoryCacheProvider(cache, null);

                // Act
                provider.Set("key1", "value1");

                // Assert
                Assert.Equal("value1", cache.Get<string>("key1"));

                // Teardown
                cache.Dispose();
            }
        }

        public class TryGetMethod
        {
            public class Throws
            {
                [Fact]
                public async Task If_MemoryCache_Is_Disposed()
                {
                    // Arrange
                    var cacheMock = new Mock<IMemoryCache>();
                    var provider = new MemoryCacheProvider(cacheMock.Object, null);

                    // Act
                    provider.Dispose();

                    // Assert
                    await Assert.ThrowsAsync<ObjectDisposedException>(
                        async () => await provider.TryGet<string>("key1"));
                }
            }

            private delegate bool TryGetValueReturns(object key, out object item);

            [Fact]
            public async Task Returns_False_When_Item_Is_Not_Cached()
            {
                // Arrange
                var cacheMock = new Mock<IMemoryCache>();
                cacheMock.Setup(m => m.TryGetValue("key1", out It.Ref<object>.IsAny))
                    .Returns(new TryGetValueReturns((object key, out object item) =>
                    {
                        item = default;
                        return false;
                    }));

                var provider = new MemoryCacheProvider(cacheMock.Object, null);

                // Act
                var result = await provider.TryGet<string>("key1");

                // Assert
                Assert.False(result.Succeeded);
                Assert.Null(result.Value);
            }

            [Fact]
            public async Task Returns_False_When_Item_Is_Of_Wrong_Type()
            {
                // Arrange
                var cacheMock = new Mock<IMemoryCache>();
                cacheMock.Setup(m => m.TryGetValue("key1", out It.Ref<object>.IsAny))
                    .Returns(new TryGetValueReturns((object key, out object item) =>
                    {
                        item = 1.0;
                        return true;
                    }));

                var provider = new MemoryCacheProvider(cacheMock.Object, null);

                // Act
                var result = await provider.TryGet<string>("key1");

                // Assert
                Assert.False(result.Succeeded);
                Assert.Null(result.Value);
            }

            [Fact]
            public async Task Returns_True_When_Item_Of_Correct_Type_Is_In_Cache()
            {
                // Arrange
                var cacheMock = new Mock<IMemoryCache>();
                cacheMock.Setup(m => m.TryGetValue("key1", out It.Ref<object>.IsAny))
                    .Returns(new TryGetValueReturns((object key, out object item) =>
                    {
                        item = "abc321";
                        return true;
                    }));

                var provider = new MemoryCacheProvider(cacheMock.Object, null);

                // Act
                var result = await provider.TryGet<string>("key1");

                // Assert
                Assert.True(result.Succeeded);
                Assert.Equal("abc321", result.Value);
            }
        }

        public class RemoveMethod
        {
            public class Throws
            {
                [Fact]
                public async Task If_MemoryCache_Is_Disposed()
                {
                    // Arrange
                    var cacheMock = new Mock<IMemoryCache>();
                    var provider = new MemoryCacheProvider(cacheMock.Object, null);

                    // Act
                    provider.Dispose();

                    // Assert
                    await Assert.ThrowsAsync<ObjectDisposedException>(
                        async () => await provider.Remove("key1"));
                }
            }

            [Fact]
            public async Task Will_Not_Throw_When_Item_Does_Not_Exist()
            {
                // Arrange
                var cache = new MemoryCache(new MemoryCacheOptions());
                var provider = new MemoryCacheProvider(cache, null);

                // Act
                await provider.Remove("key1");

                // Teardown
                cache.Dispose();
            }

            [Fact]
            public void Will_Remove_Item_When_Such_Exist()
            {
                // Arrange
                var cacheMock = new Mock<IMemoryCache>();
                var provider = new MemoryCacheProvider(cacheMock.Object, null);

                // Act
                provider.Remove("key1").Wait();

                // Assert
                cacheMock.Verify(m => m.Remove("key1"), Times.Once);
            }
        }
    }
}