namespace Cached.Tests.Benchmark
{
    using System;
    using BenchmarkDotNet.Running;

    internal class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<SimpleCacheLoad>();

            Console.WriteLine("Press any key to quit..");
            Console.ReadKey();
        }
    }
}