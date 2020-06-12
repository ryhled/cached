namespace Cached.Demo.NetCore.Web.Pages
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Caching;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class CachedFunctionModel : PageModel
    {
        private readonly ICached<string, int> _fakeServiceDates;

        public CachedFunctionModel(ICached<string, int> fakeServiceDates)
        {
            _fakeServiceDates = fakeServiceDates;
        }

        [BindProperty] public string CachedValue { get; set; }
        [BindProperty] public string TimeConsumed { get; set; }

        public async Task OnGet(int key = 0)
        {
            var watch = Stopwatch.StartNew();

            CachedValue = await _fakeServiceDates.GetOrFetchAsync(key);

            watch.Stop();
            TimeConsumed = watch.ElapsedMilliseconds + " ms";
        }
    }
}