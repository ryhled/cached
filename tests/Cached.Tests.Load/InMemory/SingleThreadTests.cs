namespace Cached.Tests.Load.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Cached.InMemory;

    public static class SingleThreadTests
    {
        public static void RunSingleThreadTests(this InMemoryCacher cacher, int iterations)
        {
            // Sequential miss test
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"[SEQ MISS TEST] Running {iterations} sequential cache misses on different keys..");

            var allMissValues = new List<int>();
            var allMissTest = Stopwatch.StartNew();
            for (var i = 0; i < iterations; ++i)
            {
                allMissValues.Add(cacher.GetOrFetchAsync("RunSingleThreadTests1_" + i, _ => Task.FromResult(i)).Result);
            }
            allMissTest.Stop();

            if (allMissValues.GroupBy(v => v).Any(g => g.Count() > 1))
                throw new InvalidOperationException("All values should have been different");

            Console.WriteLine($"[SEQ MISS TEST] Test done, took {allMissTest.ElapsedMilliseconds} ms to run.");

            // Sequential hit test
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"[SEQ HIT TEST] Running {iterations} sequential cache hits (only first request miss)..");

            var allHitValues = new List<int>();
            var allHitTest = Stopwatch.StartNew();
            for (var i = 0; i < iterations; ++i)
            {
                allHitValues.Add(cacher.GetOrFetchAsync("RunSingleThreadTests_Hit", _ => Task.FromResult(i)).Result);
            }
            allHitTest.Stop();

            if (allHitValues.Any(v => v != 0))
                throw new InvalidOperationException("All values should have been same");

            Console.WriteLine($"[SEQ HIT TEST] Test done, took {allHitTest.ElapsedMilliseconds} ms to run.");
        }
    }
}
