﻿namespace Cached.Tests.Integration.InMemory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Caching;
    using Memory;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public sealed class InMemoryCacherIntegrationTests : IDisposable
    {
        public InMemoryCacherIntegrationTests()
        {
            var services = new ServiceCollection();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            services.AddSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()));
            services.AddCached(options => options.AddMemoryCaching());

            ServiceProvider scope = services.BuildServiceProvider();
            _injectedCacher = scope.GetService<ICache<IMemory>>();
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
        }

        private readonly IMemoryCache _memoryCache;
        private readonly ICache<IMemory> _injectedCacher;

        [Fact]
        public async Task Instance_Created_Through_MemoryCacher_New_Runs_With_Provided_Cache()
        {
            // Arrange
            var cacher = MemoryCacheHandler.New(_memoryCache);

            // Act
            var result = await cacher.GetOrFetchAsync("name", _ => Task.FromResult("sven"));

            // Assert
            Assert.Equal("sven", result);
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
                var result = await _injectedCacher.GetOrFetchAsync("Only_Fetch_Once_During_Stampede_Key", FetchTask);
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

        [Fact]
        public async Task Will_Fetch_Async_Function_Correctly()
        {
            // Arrange, Act
            var result = await _injectedCacher.GetOrFetchAsync("a", async _ =>
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
            var result1 = await _injectedCacher.GetOrFetchAsync("a", _ => Task.FromResult("first_fetch"));
            var result2 = await _injectedCacher.GetOrFetchAsync("aa", _ => Task.FromResult("second_fetch"));
            var result3 = await _injectedCacher.GetOrFetchAsync("a", _ => Task.FromResult("third_fetch"));
            var result4 = await _injectedCacher.GetOrFetchAsync("aa", _ => Task.FromResult("fourth_fetch"));

            // Assert
            Assert.Equal("first_fetch", result1);
            Assert.Equal("second_fetch", result2);
            Assert.Equal("first_fetch", result3);
            Assert.Equal("second_fetch", result4);
        }
    }
}