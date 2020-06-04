namespace Cached.Tests.InMemory
{
    using System;
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

        public class ReturnInstance
        {
            [Fact]
            public void As_ICacher_Typed_Object()
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
    }
}
