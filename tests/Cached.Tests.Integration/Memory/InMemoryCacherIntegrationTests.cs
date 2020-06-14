namespace Cached.Tests.Integration.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Caching;
    using MemoryCache;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public sealed class InMemoryCacherIntegrationTests : IDisposable
    {
        public InMemoryCacherIntegrationTests()
        {
            var services = new ServiceCollection();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
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

        private static async Task<string> Only_Does_Fetch_Once_During_Stampede__Async_FetchTask(string key)
        {
            await Task.Delay(50);
            return DateTimeOffset.UtcNow.ToString();
        }

        [Fact]
        public async Task Always_Fetch_On_Cache_Miss()
        {
            // Arrange
            var tasks = new List<Task<string>>();
            for (var i = 0; i < 50; i++)
            {
                tasks.Add(_injectedCacher.GetOrFetchAsync(
                    i.ToString(),
                    provider => Task.FromResult(Guid.NewGuid().ToString())));
            }

            // Act
            var taskResults = await Task.WhenAll(tasks);

            //Assert
            Assert.True(taskResults.GroupBy(v => v).All(g => g.Count() == 1));
        }

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
        public async Task Only_Does_Fetch_Once_During_Stampede__Async()
        {
            // Arrange
            const int taskCount = 100;

            async Task<string> StampedeTask() => await _injectedCacher.GetOrFetchAsync(
                "Only_Fetch_Once_During_Stampede_Key",
                Only_Does_Fetch_Once_During_Stampede__Async_FetchTask);

            var tasks = Enumerable.Repeat(StampedeTask(), taskCount);

            // Act
            var watch = Stopwatch.StartNew();
            var tasksResult = await Task.WhenAll(tasks);
            watch.Stop();

            // Assert
            Assert.All(tasksResult, value => tasksResult[0].Equals(value));
            Assert.True(watch.ElapsedMilliseconds < 100);
        }

        [Fact]
        public async Task Only_Does_Fetch_Once_During_Stampede__Parallel()
        {
            // Arrange
            var fetchCounter = 0;
            var callCounter = 0;
            const int taskCount = 100;

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