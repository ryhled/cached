namespace Cached.Tests.Caching
{
    using System;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Moq;
    using Xunit;

    public class CachedTests
    {
        public interface IFakeCacheProvider : ICacheProvider
        {
        }

        public sealed class Constructor
        {
            public sealed class Throws
            {
                [Fact]
                public void If_Cacher_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new Cached<object, object, IFakeCacheProvider>(
                            null,
                            o => "",
                            (s, o) => Task.FromResult(o)));
                }

                [Fact]
                public void If_FetchFactory_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new Cached<object, object, IFakeCacheProvider>(
                            new Mock<ICache<IFakeCacheProvider>>().Object,
                            o => "",
                            null));
                }

                [Fact]
                public void If_KeyFactory_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new Cached<object, object, IFakeCacheProvider>(
                            new Mock<ICache<IFakeCacheProvider>>().Object,
                            null,
                            (key, arg) => Task.FromResult(arg)));
                }
            }
        }

        public sealed class GetOrFetchAsyncMethod
        {
            [Fact]
            public async Task Fetch_Value_From_Cacher()
            {
                // Arrange
                var cacherMock = new Mock<ICache<IFakeCacheProvider>>();
                cacherMock.Setup(c => c.GetOrFetchAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<string>>>()))
                    .Returns((string key, Func<string, Task<string>> fetch) =>
                        Task.FromResult(fetch(key).Result + key));

                var memoryCached = new Cached<string, int, ICacheProvider>(
                    cacherMock.Object,
                    arg => "key_" + arg,
                    (key, arg) => Task.FromResult("fetch_" + arg));

                // Act
                var response = await memoryCached.GetOrFetchAsync(22);

                // Assert
                Assert.Equal("fetch_22key_22", response);
                cacherMock.Verify(
                    c =>
                        c.GetOrFetchAsync(It.IsAny<string>(),
                            It.IsAny<Func<string, Task<string>>>()),
                    Times.Once);
            }

            [Fact]
            public async Task Passes_Key_To_FetchFactory()
            {
                // Arrange
                var cacherMock = new Mock<ICache<IFakeCacheProvider>>();
                var keyFromCachedCall = string.Empty;
                cacherMock.Setup(c => c.GetOrFetchAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<string>>>()))
                    .Returns((string key, Func<string, Task<string>> fetch) =>
                    {
                        keyFromCachedCall = key;
                        return Task.FromResult(key);
                    });

                var memoryCached = new Cached<string, int, IFakeCacheProvider>(
                    cacherMock.Object,
                    arg => arg.ToString(),
                    (key, arg) => Task.FromResult(key));

                // Act
                var response = await memoryCached.GetOrFetchAsync(221);

                // Assert
                Assert.Equal("221", response);
                Assert.Equal("221", keyFromCachedCall);
            }
        }
    }
}