namespace Cached.Demo.NetCore.Web.Pages
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Caching;
    using MemoryCache;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Services;

    public class CacherModel : PageModel
    {
        private readonly ICache<IMemory> _cache;
        private readonly IFakeService _fakeService;

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