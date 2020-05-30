
using System;
using System.Threading.Tasks;

namespace Cached.Demo.Load
{
    public static class LockLoadTest
    {
        public static void Run()
        {
            Console.WriteLine("Running load test on lock..");
            var lock = new SemaphoreSlimLock();
        }

        private static Task LockTask()
        {
            
        }
    }
}
