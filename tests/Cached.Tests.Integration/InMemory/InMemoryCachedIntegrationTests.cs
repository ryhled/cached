namespace Cached.Tests.Integration.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Configuration;
    using Microsoft.Extensions.Caching.Memory;
    using Xunit;

    public sealed class InMemoryCachedIntegrationTests : IDisposable
    {
        public InMemoryCachedIntegrationTests()
        {
            var options = new MemoryCacheOptions();
            _memoryCache = new MemoryCache(options);
            IInMemoryCacher inMemoryCacher =
                InMemoryCacher.New(new CachedSettings(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2)), _memoryCache);
            _inMemoryCached = new InMemoryCached<string, int>(inMemoryCacher, i => i + "_key", async i =>
            {
                await Task.Delay(10);
                ++_hitCounter;
                return i + "_" + _hitCounter + "_fetch";
            });
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
        }

        private readonly IMemoryCache _memoryCache;
        private readonly ICached<string, int> _inMemoryCached;
        private int _hitCounter;

        [Fact]
        public async Task InMemoryCached_will_fetch_or_cache_based_on_key()
        {
            // Arrange, Act
            var result1 = await _inMemoryCached.GetOrFetchAsync(1);
            var result2 = await _inMemoryCached.GetOrFetchAsync(2);
            var result3 = await _inMemoryCached.GetOrFetchAsync(1);
            var result4 = await _inMemoryCached.GetOrFetchAsync(2);

            // Assert
            Assert.Equal("1_1_fetch", result1);
            Assert.Equal("2_2_fetch", result2);
            Assert.Equal("1_1_fetch", result3);
            Assert.Equal("2_2_fetch", result4);
        }
    }
}