namespace Cached.Tests.Integration.InMemory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cached.InMemory;
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
                new MemoryCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)});
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
        }

        private readonly IMemoryCache _memoryCache;
        private readonly IInMemoryCacher _inMemoryCacher;

        [Fact]
        public async Task Default_InMemoryCacher_Instances_Share_Cache()
        {
            // Arrange
            InMemoryCacher cacher1 = InMemoryCacher.Default();
            InMemoryCacher cacher2 = InMemoryCacher.Default();

            // Act
            var result1 = await cacher1.GetOrFetchAsync("name", _ => Task.FromResult("sven"));
            var result2 = await cacher2.GetOrFetchAsync("name", _ => Task.FromResult("oscar"));

            // Assert
            Assert.NotEqual(cacher1, cacher2);
            Assert.Equal("sven", result1);
            Assert.Equal("sven", result2);
        }

        [Fact]
        public async Task Will_Fetch_Async_Function_Correctly()
        {
            // Arrange, Act
            var result = await _inMemoryCacher.GetOrFetchAsync("a", async _ =>
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
            var result1 = await _inMemoryCacher.GetOrFetchAsync("a", _ => Task.FromResult("first_fetch"));
            var result2 = await _inMemoryCacher.GetOrFetchAsync("aa", _ => Task.FromResult("second_fetch"));
            var result3 = await _inMemoryCacher.GetOrFetchAsync("a", _ => Task.FromResult("third_fetch"));
            var result4 = await _inMemoryCacher.GetOrFetchAsync("aa", _ => Task.FromResult("fourth_fetch"));

            // Assert
            Assert.Equal("first_fetch", result1);
            Assert.Equal("second_fetch", result2);
            Assert.Equal("first_fetch", result3);
            Assert.Equal("second_fetch", result4);
        }

        [Fact]
        public async Task Only_Does_Fetch_Once_During_Stampede()
        {
            // Arrange
            var fetchCounter = 0;
            var callCounter = 0;
            const int taskCount = 100; // the number of clients trying to access same cache entry simultaneously.

            async Task<int> FetchTask(string key)
            {
                await Task.Delay(50);
                return Interlocked.Increment(ref fetchCounter);
            }

            async Task StampedeTask()
            {
                var result = await _inMemoryCacher.GetOrFetchAsync("Only_Fetch_Once_During_Stampede_Key", FetchTask);
                Assert.Equal(1, result);
                Interlocked.Increment(ref callCounter);
            }

            // Act
            Parallel.For(0, taskCount, async nr => await StampedeTask());

            while (callCounter < taskCount)
            {
                await Task.Delay(10);
            }

            // Assert
            Assert.Equal(1, fetchCounter);
            Assert.Equal(taskCount, callCounter);
        }
    }
}