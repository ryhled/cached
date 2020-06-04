namespace Cached.Tests.Benchmark.Simple
{
    using System;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Engines;
    using BenchmarkDotNet.Jobs;
    using Caching;
    using InMemory;
    using Microsoft.Extensions.Caching.Memory;

    [SimpleJob, IterationTime(500)]
    public class CacheHitsTests
    {
        public CacheHitsTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _cacher = InMemoryCacher.New(_cache);
        }

        private readonly ICacher<IInMemory> _cacher;
        private readonly IMemoryCache _cache;

        [Benchmark]
        public async Task Cached() => await _cacher.GetOrFetchAsync(
            "CacheHitsTests_Cached",
            _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));

        [Benchmark]
        public async Task MemoryCache() => await _cache.GetOrCreateAsync(
            "CacheHitsTests_MemoryCache",
            _ => Task.FromResult(DateTimeOffset.UtcNow.ToString()));
    }
}
