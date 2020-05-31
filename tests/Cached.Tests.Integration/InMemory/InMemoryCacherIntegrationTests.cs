namespace Cached.Tests.Integration.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Configuration;
    using Microsoft.Extensions.Caching.Memory;
    using Xunit;

    public sealed class InMemoryCacherIntegrationTests : IDisposable
    {
        public InMemoryCacherIntegrationTests()
        {
            var options = new MemoryCacheOptions();
            _memoryCache = new MemoryCache(options);
            _inMemoryCacher = InMemoryCacher.New(
                _memoryCache,
                new CachedSettings(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2)));
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
        }

        private readonly IMemoryCache _memoryCache;
        private readonly IInMemoryCacher _inMemoryCacher;

        [Fact]
        public async Task Will_Fetch_Async_Function_Correctly()
        {
            // Arrange, Act
            var result = await _inMemoryCacher.GetOrFetchAsync("a", async () =>
            {
                await Task.Delay(1);
                return "fetch";
            });

            // Assert
            Assert.Equal("fetch", result);
        }

        [Fact]
        public async Task Will_Fetch_Or_Cache_Based_On_Key()
        {
            // Arrange, Act
            var result1 = await _inMemoryCacher.GetOrFetchAsync("a", () => Task.FromResult("first_fetch"));
            var result2 = await _inMemoryCacher.GetOrFetchAsync("aa", () => Task.FromResult("second_fetch"));
            var result3 = await _inMemoryCacher.GetOrFetchAsync("a", () => Task.FromResult("third_fetch"));
            var result4 = await _inMemoryCacher.GetOrFetchAsync("aa", () => Task.FromResult("fourth_fetch"));

            // Assert
            Assert.Equal("first_fetch", result1);
            Assert.Equal("second_fetch", result2);
            Assert.Equal("first_fetch", result3);
            Assert.Equal("second_fetch", result4);
        }

        [Fact]
        public async Task Default_InMemoryCacher_Instances_Share_Cache()
        {
            // Arrange
            InMemoryCacher cacher1 = InMemoryCacher.Default();
            InMemoryCacher cacher2 = InMemoryCacher.Default();

            // Act
            var result1 = await cacher1.GetOrFetchAsync("name", () => Task.FromResult("sven"));
            var result2 = await cacher2.GetOrFetchAsync("name", () => Task.FromResult("oscar"));

            // Assert
            Assert.NotEqual(cacher1, cacher2);
            Assert.Equal("sven", result1);
            Assert.Equal("sven", result2);
        }
    }
}