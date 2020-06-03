namespace Cached.Demo.Net.Pages
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Caching;
    using InMemory;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Services;

    public class CacherModel : PageModel
    {
        private readonly ICacher<IInMemory> _memoryCacher;
        private readonly IFakeService _fakeService;

        public CacherModel(
            ICacher<IInMemory> cached,
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