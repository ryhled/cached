using System;

namespace Cached.Tests.Benchmark
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Running;

    class Program
    {
        static async Task Main(string[] args)
        {
            var simpleMissSummary = BenchmarkRunner.Run<SimpleCacheLoad>();
            
            Console.WriteLine("Press any key to quit..");
            Console.ReadKey();
        }
    }
}
