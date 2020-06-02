using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cached.Tests.Load
{
    using System.Diagnostics;
    using InMemory;

    public static class SingleThreadTests
    {
        public static async Task RunSingleThreadTests(this InMemoryCacher cacher, int iterations)
        {
            // Sequential miss test

            Console.WriteLine($"[Cache miss test] Running {iterations} sequential cache misses on different keys..");

            var allMissValues = new List<int>();
            var allMissTest = Stopwatch.StartNew();
            for (var i = 0; i < iterations; ++i)
            {
                allMissValues.Add(cacher.GetOrFetchAsync(i.ToString(), () => Task.FromResult(i)).Result);
            }
            allMissTest.Stop();

            if (allMissValues.GroupBy(v => v).Any(g => g.Count() > 1))
                throw new InvalidOperationException("All values should have been different");

            Console.WriteLine($"[Cache miss test] Test done, took {allMissTest.ElapsedMilliseconds} ms to run.");

            // Sequential hit test

            Console.WriteLine(Environment.NewLine, $"[Cache hit test] Running {iterations} sequential cache hits (only first request miss)..");

            var allHitValues = new List<int>();
            var allHitTest = Stopwatch.StartNew();
            for (var i = 0; i < iterations; ++i)
            {
                allHitValues.Add(cacher.GetOrFetchAsync("a", () => Task.FromResult(i)).Result);
            }
            allHitTest.Stop();

            if (allHitValues.Any(v => v != 0))
                throw new InvalidOperationException("All values should have been same");

            Console.WriteLine($"[Cache miss test] Test done, took {allHitTest.ElapsedMilliseconds} ms to run.");
        }
    }
}
