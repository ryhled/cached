namespace Cached.Tests.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    public class InMemoryProviderTests
    {
        public class Constructor
        {

            public class Throws
            {
                [Fact]
                public void If_MemoryCache_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new InMemoryProvider(
                            null,
                            new MemoryCacheEntryOptions()));
                }
            }

            public class DoesNotThrow
            {
                [Fact]
                public void If_Option_Argument_Is_Null()
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new InMemoryProvider(new Mock<IMemoryCache>().Object, null);
                }
            }
        }

        public class TryGetFromCacheMethod
        {
            [Fact]
            public async Task Return_False_If_Item_Does_Not_Exist()
            {
                // Arrange
                var cache = new MemoryCache(new MemoryCacheOptions());
                var provider = new InMemoryProvider(cache, default);

                // Act, Assert
                Assert.False(await provider.TryGetFromCache("Return_False_If_Item_Does_Not_Exist", out string item));
                Assert.Null(item);

                // Teardown
                cache.Dispose();
            }

            [Fact]
            public async Task Return_False_If_Item_Is_Of_Wrong_Type()
            {
                // Arrange
                var cache = new MemoryCache(new MemoryCacheOptions());
                var provider = new InMemoryProvider(cache, default);
                cache.Set("Return_False_If_Item_Is_Of_Wrong_Type", 2);

                // Act, Assert
                Assert.False(await provider.TryGetFromCache("Return_False_If_Item_Is_Of_Wrong_Type", out string item));
                Assert.Null(item);

                // Teardown
                cache.Dispose();
            }

            [Fact]
            public async Task Return_True_If_Item_Exist_And_Is_Matching_Type()
            {
                // Arrange
                var cache = new MemoryCache(new MemoryCacheOptions());
                var provider = new InMemoryProvider(cache, default);
                cache.Set("Return_False_If_Item_Is_Of_Wrong_Type", "2");

                // Act, Assert
                Assert.True(await provider.TryGetFromCache("Return_False_If_Item_Is_Of_Wrong_Type", out string item));
                Assert.Equal("2", item);

                // Teardown
                cache.Dispose();
            }
        }

        public class WriteToCacheMethod
        {
            [Fact]
            public async Task Writes_Value_With_Correct_Type_And_Key()
            {
                // Arrange
                var cache = new MemoryCache(new MemoryCacheOptions());
                var provider = new InMemoryProvider(cache, default);

                // Act
                await provider.WriteToCache(
                    "Writes_Value_With_Correct_Type_And_Key",
                    new InMemoryProviderTestClass {Number = 2.12});
                var item = cache.Get<InMemoryProviderTestClass>("Writes_Value_With_Correct_Type_And_Key");
                
                // Assert
                Assert.NotNull(item);
                Assert.Equal(2.12, item.Number);

                // Teardown
                cache.Dispose();
            }
        }

        internal class InMemoryProviderTestClass
        {
            public double Number { get; set; }
        }
    }
}
