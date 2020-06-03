namespace Cached.Demo.Net.Pages
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading.Tasks;
    using InMemory;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Services;

    public class CacherModel : PageModel
    {
        private readonly IInMemoryCacher _cached;
        private readonly IFakeService _fakeService;

        public CacherModel(
            IInMemoryCacher cached,
            IFakeService fakeService)
        {
            _cached = cached;
            _fakeService = fakeService;
        }

        [BindProperty] public string CachedValue { get; set; }
        [BindProperty] public string TimeConsumed { get; set; }

        public async Task OnGet(string key = "_")
        {
            var watch = Stopwatch.StartNew();

            CachedValue = await _cached.GetOrFetchAsync(key, _fakeService.Get);

            watch.Stop();
            TimeConsumed = watch.ElapsedMilliseconds + " ms";
        }
    }
}