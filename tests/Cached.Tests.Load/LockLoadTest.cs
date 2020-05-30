
using Cached.Locking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cached.Tests.Load
{
    public static class LockLoadTest
    {
        public static async Task Run()
        {
            Console.WriteLine("Running load test on lock..");
            var semaphoreSlimLock = new SemaphoreSlimLock();
            var lockTasks = new List<Task>();
            
            for(int i = 0; i < 100; ++i)
            {
                lockTasks.Add(LockTask(semaphoreSlimLock, "lkt1_" + i));
            }

            var watch = Stopwatch.StartNew();
            await Task.WhenAll(lockTasks);
            watch.Stop();
            Console.WriteLine("Execution time: " + watch.ElapsedMilliseconds + " ms");
        }

        private static async Task LockTask(SemaphoreSlimLock semaphoreSlimLock, string lockKey)
        {
            var startTime = DateTimeOffset.UtcNow.AddMinutes(2);
            while (startTime > DateTimeOffset.UtcNow)
            {
                using (await semaphoreSlimLock.LockAsync(lockKey))
                {
                    await Task.Delay(10);
                }
            }
        }
    }
}
