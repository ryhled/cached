namespace Cached.Tests.Benchmark
{
    using System;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using Caching;
    using InMemory;
    using Microsoft.Extensions.Caching.Memory;

    [SimpleJob, IterationTime(200), MemoryDiagnoser, EtwProfiler, ConcurrencyVisualizerProfiler]
    public class SimpleCacheLoad
    {
        public SimpleCacheLoad()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _cacher = InMemoryCacher.New(_cache);
        }

        private readonly ICacher<IInMemory> _cacher;
        private readonly IMemoryCache _cache;

        [Benchmark]
        public async Task Cached_Hit() => await _cacher.GetOrFetchAsync(
            "CacheHitsTests_Cached",
            _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));

        [Benchmark]
        public async Task MemoryCache_Hit() => await _cache.GetOrCreateAsync(
            "CacheHitsTests_MemoryCache",
            _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));

        private static long counter = 0;

        [Benchmark]
        public async Task Cached_Miss() => await _cacher.GetOrFetchAsync(
            (++counter).ToString(),
            _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));

        [Benchmark]
        public async Task MemoryCache_Miss() => await _cache.GetOrCreateAsync(
            (++counter).ToString(),
            _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));

        
    }
}
