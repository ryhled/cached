using System.Web.Mvc;

namespace Cached.Demo.Net461.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Caching;
    using Memory;

    public class HomeController : Controller
    {
        private static readonly ICache<IMemory> Cache = MemoryCacheHandler.New();

        public ActionResult Index()
        {
            ViewBag.Value = Cache.GetOrFetchAsync(
                "key1", 
                provider => Task.FromResult(DateTimeOffset.UtcNow.ToString())
            ).Result;

            return View();
        }
    }
}