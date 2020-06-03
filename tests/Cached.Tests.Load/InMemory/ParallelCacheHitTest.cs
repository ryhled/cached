namespace Cached.Tests.Load.InMemory
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Caching;

    public static class ParallelCacheHitTest
    {
        public static void RunParallelCacheHitTest(this ICacher<InMemory> cacher, int hits)
        {
            Console.WriteLine($"[PARALLEL HIT TEST] Running {hits} cache hits in parallel on same key..");

            // Run fetches in parallel
            var watch = Stopwatch.StartNew();
            Parallel.For(
                0,
                hits,
                i =>
                {
                    var res = cacher.GetOrFetchAsync("RunParallelCacheHitTest", RunAllHitTestTask).Result;
                    if (res != 1)
                    {
                        throw new ArgumentException("res should have been 1 for all requests (it should be cached after first)..");
                    }
                });
            watch.Stop();

            if (_runAllHitTestCounter != 1)
            {
                throw new ArgumentException("Test task should not have been hit more (or less than) once.");
            }

            Console.WriteLine($"[PARALLEL HIT TEST] All hits completed, took {watch.ElapsedMilliseconds} ms to run (this seems much higher latency than miss, even though just doing MemoryCache Get under hood?).");
        }

        private static int _runAllHitTestCounter = 0;

        /// <summary>
        ///     Since we are hitting same cache key this is thread safe.
        ///     This means no need to interlock, they are sun sequentially.
        /// </summary>
        private static Task<int> RunAllHitTestTask(string key) => Task.FromResult(++_runAllHitTestCounter);
    }
}
