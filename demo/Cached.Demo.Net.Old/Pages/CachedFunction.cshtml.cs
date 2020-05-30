using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cached.Demo.Net.Pages
{
    public class CachedFunctionModel : PageModel
    {
        private readonly ICached<string, string> _cached;

        public CachedFunctionModel(ICached<string, string> cached)
        {
            _cached = cached;
        }

        [BindProperty] public string CachedValue { get; set; }
        [BindProperty] public string TimeConsumed { get; set; }

        public async Task OnGet(string key = "_")
        {
            var watch = Stopwatch.StartNew();

            CachedValue = await _cached.GetOrFetchAsync(key);

            watch.Stop();
            TimeConsumed = watch.ElapsedMilliseconds + " ms";
        }
    }
}
