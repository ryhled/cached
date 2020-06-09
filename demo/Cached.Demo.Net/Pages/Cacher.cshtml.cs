namespace Cached.Demo.Net.Pages
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Caching;
    using Memory;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Services;

    public class CacherModel : PageModel
    {
        private readonly IFakeService _fakeService;
        private readonly ICache<IMemory> _cache;

        public CacherModel(
            ICache<IMemory> cache,
            IFakeService fakeService)
        {
            _cache = cache;
            _fakeService = fakeService;
        }

        [BindProperty] public string CachedValue { get; set; }
        [BindProperty] public string TimeConsumed { get; set; }

        public async Task OnGet(string key = "_")
        {
            var watch = Stopwatch.StartNew();

            CachedValue = await _cache.GetOrFetchAsync(key, _fakeService.Get);

            watch.Stop();
            TimeConsumed = watch.ElapsedMilliseconds + " ms";
        }
    }
}