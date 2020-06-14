namespace Cached.Demo.NetCore.Cmd
{
    using System;
    using System.Threading.Tasks;
    using MemoryCache;

    internal class Program
    {
        private static async Task Main(string[] args)
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

                var result = await cache.GetOrFetchAsync(
                    key.Key.ToString(),
                    provider => Task.FromResult(DateTimeOffset.UtcNow.ToString()));

                Console.WriteLine($"Value is: {result}");
            }
        }
    }
}