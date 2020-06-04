using System;

namespace Cached.Tests.Load
{
    using System.Threading.Tasks;
    using Cached.InMemory;
    using InMemory;
    using Microsoft.Extensions.Caching.Memory;

    class Program
    {
        static async Task Main(string[] args)
        {
            var cacher = InMemoryCacher.New(new MemoryCache(new MemoryCacheOptions()));

            Console.WriteLine("----- [InMemory] Running sequential load tests -----");
            cacher.RunSingleThreadTests(10000);

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("----- [InMemory] Running concurrency load tests -----");
            cacher.RunParallelCacheMissTest(10000); // Not using async since parallel do not await async properly..
            cacher.RunParallelCacheHitTest(10000); // Not using async since parallel do not await async properly..
            await cacher.RunAsyncCacheMissTest(10000);
            await cacher.RunAsyncCacheHitTest(10000);

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("All tests completed, press any key to quit..");
            Console.ReadKey();
        }
    }
}
