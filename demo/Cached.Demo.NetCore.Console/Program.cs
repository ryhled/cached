namespace Cached.Demo.NetCore.Console
{
    using System;
    using System.Threading.Tasks;
    using Memory;

    class Program
    {
        static void Main(string[] args)
        {
            using var cache = MemoryCacheHandler.New();

            while (true)
            {
                Console.WriteLine("Press any key, used as cache key, to get data from cache (or 'q' to quit)");
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.Q)
                {
                    break;
                }

                var result = cache.GetOrFetchAsync(
                    key.Key.ToString(),
                    provider => Task.FromResult(DateTimeOffset.UtcNow.ToString()))
                    .Result;

                Console.WriteLine($"Value is: {result}");
            }
        }
    }
}
