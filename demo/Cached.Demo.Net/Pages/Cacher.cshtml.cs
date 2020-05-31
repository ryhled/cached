namespace Cached.Demo.Net.Pages
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading.Tasks;
    using InMemory;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class CacherModel : PageModel
    {
        private readonly IInMemoryCacher _cached;

        public CacherModel(IInMemoryCacher cached)
        {
            _cached = cached;
        }

        [BindProperty] public string CachedValue { get; set; }
        [BindProperty] public string TimeConsumed { get; set; }

        public async Task OnGet(string key = "_")
        {
            var watch = Stopwatch.StartNew();

            CachedValue = await _cached.GetOrFetchAsync(key, () => GetSlowDateTime(key));

            watch.Stop();
            TimeConsumed = watch.ElapsedMilliseconds + " ms";
        }

        private async Task<string> GetSlowDateTime(string key)
        {
            await Task.Delay(500);
            return DateTime.Now.ToString(CultureInfo.InvariantCulture) + $" [{key}]";
        }
    }
}