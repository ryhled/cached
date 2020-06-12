namespace Cached.Tests.Memory
{
    using System;
    using Cached.Memory;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    public class MemoryCacheHandlerTests
    {
        public class NewMethod
        {
            public class Throws
            {
                [Fact]
                public void If_MemoryCache_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        MemoryCacheHandler.New(null, new MemoryCacheEntryOptions()));
                }
            }

            [Fact]
            public void Creates_Instance()
            {
                // Arrange, Act
                var result = MemoryCacheHandler.New(new Mock<IMemoryCache>().Object);

                // Assert
                Assert.NotNull(result);
            }

            [Fact]
            public void Creates_Instance_WithOptions_Argument()
            {
                // Arrange, Act
                var result = MemoryCacheHandler.New(new Mock<IMemoryCache>().Object, new MemoryCacheEntryOptions());

                // Assert
                Assert.NotNull(result);
            }
        }
    }
}