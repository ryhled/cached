namespace Cached.Tests.Benchmark
{
    using System;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using Caching;
    using MemoryCache;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;

    [SimpleJob]
    [IterationTime(200)]
    [MemoryDiagnoser]
    [EtwProfiler]
    [ConcurrencyVisualizerProfiler]
    public class SimpleCacheLoad
    {
        private readonly IMemoryCache _cache;

        private readonly ICached<string, string> _cached;

        private readonly ICache<IMemory> _cacher;

        private readonly IServiceProvider _serviceProvider;

        public SimpleCacheLoad()
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
        public async Task Cached_Cacher_Hit()
        {
            await _cacher.GetOrFetchAsync(
                "Cached_Cacher_Hit",
                _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));
        }

        [Benchmark]
        public async Task Cached_Cached_Hit()
        {
            await _cached.GetOrFetchAsync("Cached_Cached_Hit");
        }

        [Benchmark]
        public async Task Cached_Cached_Transient_Hit()
        {
            var cached = _serviceProvider.GetService<ICached<string, string>>();
            await cached.GetOrFetchAsync("Cached_Cached_Hit");
        }

        [Benchmark]
        public async Task MemoryCache_Hit()
        {
            await _cache.GetOrCreateAsync(
                "CacheHitsTests_MemoryCache",
                _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));
        }

        [Benchmark]
        public async Task Cached_Cacher_Miss()
        {
            await _cacher.GetOrFetchAsync(
                new Guid().ToString(),
                _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));
        }

        [Benchmark]
        public async Task Cached_Cached_Miss()
        {
            await _cached.GetOrFetchAsync(new Guid().ToString());
        }

        [Benchmark]
        public async Task Cached_Cached_Transient_Miss()
        {
            var cached = _serviceProvider.GetService<ICached<string, string>>();
            await cached.GetOrFetchAsync(new Guid().ToString());
        }

        [Benchmark]
        public async Task MemoryCache_Miss()
        {
            await _cache.GetOrCreateAsync(
                new Guid().ToString(),
                _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));
        }
    }
}