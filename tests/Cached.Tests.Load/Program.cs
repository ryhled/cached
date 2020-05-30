using System;
using System.Threading.Tasks;

namespace Cached.Tests.Load
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await LockLoadTest.Run();
        }
    }
}
