using System;

namespace Cached.Tests.Load
{
    using System.Threading.Tasks;
    using InMemory;

    class Program
    {
        static async Task Main(string[] args)
        {
            InMemoryCacher cacher = InMemoryCacher.Default();

            Console.WriteLine("----- Running test 1 (single-tread, consecutive, cache miss & hit scenarios) -----");
           await cacher.RunSingleThreadTests(10000);


           Console.WriteLine("All tests completed, pres any key to quit.");
           Console.ReadKey();
        }
    }
}
