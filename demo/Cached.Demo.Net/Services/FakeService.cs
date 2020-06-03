using System;
using System.Threading.Tasks;

namespace Cached.Demo.Net.Services
{
    public class FakeService : IFakeService
    {
        // Since function calls gets constructed keys during runtime,
        // its possible to retrieve both that actual key AND the argument it was built from.
        public async Task<string> FunctionGet(string key, int arg)
        {
            await Task.Delay(200);
            return DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + $" (key was '{key}', arg was '{arg}')";
        }

        public async Task<string> Get(string key)
        {
            await Task.Delay(200);
            return DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + $" ({key})";
        }
    }
}
