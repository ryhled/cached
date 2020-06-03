namespace Cached.Tests.Load.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Caching;

    public static class AsyncCacheMissTest
    {
        public static async Task RunAsyncCacheMissTest(this ICacher<InMemory> cacher, int hits)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"[ASYNC MISS TEST] Running {hits} cache misses asynchronously on different keys..");

            // Generating control value
            var control = 0;
            for(var i = 0; i < hits; ++i)
            {
                control += i;
            }

            // Run fetches in parallel
            var tasks = new List<Task<int>>();

            for(var i = 0; i < hits; ++i)
            {
                tasks.Add(cacher.GetOrFetchAsync("RunAsyncCacheMissTest" + i, _ => FetchTask(i)));
            }

            var watch = Stopwatch.StartNew();
            var taskResult = await Task.WhenAll(tasks);
            watch.Stop();

            if(taskResult.Length != hits)
                throw new InvalidOperationException("Found unexpected amount of hit results.");

            if (taskResult.Sum() != control)
                throw new InvalidOperationException("The internal adder and control value should have been equal");

            Console.WriteLine($"[ASYNC MISS TEST] All hits completed, took {watch.ElapsedMilliseconds} ms to run.");
        }

        private static async Task<int> FetchTask(int seed)
        {
            return seed;
        }
    }
}
