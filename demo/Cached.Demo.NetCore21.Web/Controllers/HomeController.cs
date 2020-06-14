namespace Cached.Demo.NetCore21.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Caching;
    using MemoryCache;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly ICache<IMemory> _cache;

        public HomeController(ICache<IMemory> cache)
        {
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Value =
                await _cache.GetOrFetchAsync("key1", provider => Task.FromResult(DateTimeOffset.UtcNow.ToString()));
            return View();
        }
    }
}