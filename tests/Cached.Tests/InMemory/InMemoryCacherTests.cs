namespace Cached.Tests.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Microsoft.Extensions.Caching.Memory;
    using Xunit;

    public class InMemoryCacherTests
    {
        public class Throws
        {
            [Fact]
            public void If_MemoryCache_Argument_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    InMemoryCacher.New(
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
                var instance = InMemoryCacher.New(cache);

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
                var instance = InMemoryCacher.Default();

                // Assert
                Assert.NotNull(instance);
            }

            [Fact]
            public async Task Generates_Instances_That_Shares_MemoryCache()
            {
                // Arrange
                var instance1 = InMemoryCacher.Default(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) });
                var instance2 = InMemoryCacher.Default();

                // Act
                var value1 = await instance1.GetOrFetchAsync("default_share_instance_key", key => Task.FromResult("abc"));
                var value2 = await instance2.GetOrFetchAsync("default_share_instance_key", key => Task.FromResult("cde"));

                // Assert
                Assert.Equal("abc", value1);
                Assert.Equal("abc", value2);
            }
        }
    }
}
