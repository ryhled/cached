namespace Cached.Tests.Load.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Caching;

    public static class AsyncCacheHitTest
    {
        public static async Task RunAsyncCacheHitTest(this ICacher cacher, int hits)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"[ASYNC HIT TEST] Running {hits} cache hits asynchronously on different keys..");

            // Run fetches in parallel
            var tasks = new List<Task<int>>();

            for(var i = 0; i < hits; ++i)
            {
                tasks.Add(cacher.GetOrFetchAsync("RunAsyncCacheHitTest", async _ => await FetchTask()));
            }

            var watch = Stopwatch.StartNew();
            var taskResult = await Task.WhenAll(tasks);
            watch.Stop();

            if(taskResult.Length != hits)
                throw new InvalidOperationException("Found unexpected amount of hit results.");

            if (taskResult.Sum() != hits)
                throw new InvalidOperationException("The internal adder and control value should have been equal");

            if(_callCounter != 1)
                throw new InvalidOperationException("The fetch call should have been hit exactly once.");

            Console.WriteLine($"[ASYNC HIT TEST] All hits completed, took {watch.ElapsedMilliseconds} ms to run.");
        }

        private static int _callCounter = 0;

        // Thread safe since is will be under lock in cacher (since were using same key)
        private static async Task<int> FetchTask()
        {
            await Task.Delay(1); // await 1 ms for sake of proper manner ;)
            return ++_callCounter;
        }
    }
}
