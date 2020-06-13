
namespace Cached.Tests.Benchmark
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using Caching;
    using Memory;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;

    [SimpleJob]
    [IterationTime(800)]
    [MemoryDiagnoser]
    [EtwProfiler]
    [ConcurrencyVisualizerProfiler]
    public class ComplexCacheLoad
    {
        private readonly IMemoryCache _cache;

        private readonly ICached<string, string> _cached;

        private readonly ICache<IMemory> _cacher;

        private readonly IServiceProvider _serviceProvider;

        public ComplexCacheLoad()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _cacher = MemoryCacheHandler.New(_cache);

            var services = new ServiceCollection();
            services.AddCached(config =>
            {
                config.AddMemoryCaching(options =>
                {
                    options.AddFunction<string, string>(
                        arg => arg,
                        (provider, key, arg) => Task.FromResult(DateTimeOffset.UtcNow.ToString()));
                });
            });

            _serviceProvider = services.BuildServiceProvider();
            _cached = _serviceProvider.GetService<ICached<string, string>>();
        }

        [Benchmark]
        public async Task MemoryCache_Miss()
        {
            var tasks = Enumerable.Repeat(FetchTask(
                Guid.NewGuid().ToString(),
                key => _cache.GetOrCreate(key, k => Task.FromResult(DateTimeOffset.UtcNow + k.Key.ToString()))), 100);

            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task Cached_Cacher_Miss()
        {
            var tasks = Enumerable.Repeat(FetchTask(
                Guid.NewGuid().ToString(), 
                key => _cacher.GetOrFetchAsync(key, k => Task.FromResult(DateTimeOffset.UtcNow + k))), 100);

            await Task.WhenAll(tasks);
        }

        private static async Task<string> FetchTask(string key, Func<string, Task<string>> getFunc)
        {
            for (var i = 0; i < 100; ++i)
            {
                await getFunc(key);
                await Task.Delay(3);
            }
            return DateTimeOffset.UtcNow.ToString();
        }
    }
}
