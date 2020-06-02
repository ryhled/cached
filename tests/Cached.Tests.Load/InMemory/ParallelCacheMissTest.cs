namespace Cached.Tests.Load.InMemory
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Caching;

    public static class ParallelCacheMissTest
    {
        public static void RunParallelCacheMissTest(this ICacher cacher, int hits)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"[PARALLEL MISS TEST] Running {hits} cache misses in parallel on different keys..");

            // Generating control value
            var control = 0;
            for(var i = 0; i < hits; ++i)
            {
                control += i;
            }

            // Run fetches in parallel
            var watch = Stopwatch.StartNew();
            Parallel.For(
                0,
                hits, 
                i =>
                {
                    var result = cacher.GetOrFetchAsync("RunAllMissTest" + i, () => RunAllMissTestTask(i)).Result;
                    Interlocked.Add(ref _runAllMissTestAdder, result);
                });
            watch.Stop();

            if(_runAllMissTestAdder != control)
                throw new InvalidOperationException("The internal adder and control value should have been equal");

            Console.WriteLine($"[PARALLEL MISS TEST] All hits completed, took {watch.ElapsedMilliseconds} ms to run.");
        }

        private static long _runAllMissTestAdder = 0;

        /// <summary>
        ///     We need to lock these values since we are sharing internal state ACROSS KEYS.
        ///     Cached locks only on key basis (so you can fetch two different posts simultaneously for example, without them waiting for each other to complete).
        /// </summary>
        private static Task<long> RunAllMissTestTask(long seed, bool delay = false) => Task.FromResult(seed);
    }
}
