using System;

namespace Cached.Tests.Benchmark
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;
    using Simple;

    class Program
    {
        static async Task Main(string[] args)
        {
            var simpleHitSummary = BenchmarkRunner.Run<CacheHitsTests>();
            //var simpleMissSummary = BenchmarkRunner.Run<CacheMissTests>();
            
            Console.WriteLine("Press any key to quit..");
            Console.ReadKey();
        }
    }
}
