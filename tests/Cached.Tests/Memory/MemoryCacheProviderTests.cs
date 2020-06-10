namespace Cached.Tests.Memory
{
    using System;
    using System.Globalization;
    using Cached.Memory;
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
                    Assert.Throws<ArgumentNullException>(() => new MemoryCacheProvider(null, new MemoryCacheEntryOptions()));
                }
            }
        }

        public class SetMethod
        {
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

            [Fact]
            public void Sets_Provided_Configuration_Using_Underlying_Provider()
            {
                // Arrange
                DateTimeOffset absoluteExpiration = DateTimeOffset.ParseExact(
                    "2022-01-02T23:01:22-11:22",
                    "yyyy-MM-dd'T'HH:mm:ss.FFFK",
                    CultureInfo.InvariantCulture);

                var cacheMock = new Mock<IMemoryCache>();
                var config = new MemoryCacheEntryOptions { AbsoluteExpiration = absoluteExpiration };
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
        }

        public class TryGetMethod
        {

        }

        public class RemoveMethod
        {

        }
    }
}
