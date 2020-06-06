namespace Cached.Demo.Net.Pages
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using InMemory;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Services;

    public class CacherModel : PageModel
    {
        private readonly IFakeService _fakeService;
        private readonly IInMemoryCacher _memoryCacher;

        public CacherModel(
            IInMemoryCacher cached,
            IFakeService fakeService)
        {
            _memoryCacher = cached;
            _fakeService = fakeService;
        }

        [BindProperty] public string CachedValue { get; set; }
        [BindProperty] public string TimeConsumed { get; set; }

        public async Task OnGet(string key = "_")
        {
            var watch = Stopwatch.StartNew();

            CachedValue = await _memoryCacher.GetOrFetchAsync(key, _fakeService.Get);

            watch.Stop();
            TimeConsumed = watch.ElapsedMilliseconds + " ms";
        }
    }
}