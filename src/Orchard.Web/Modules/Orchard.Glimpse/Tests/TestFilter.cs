using System;
using System.Web.Mvc;
using Orchard.Caching.Services;
using Orchard.Mvc.Filters;

namespace Orchard.Glimpse.Tests
{
    public class TestFilter : FilterProvider, IActionFilter
    {
        private readonly ICacheService _cacheService;

        public TestFilter(ICacheService cacheService) {
            _cacheService = cacheService;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) {
            _cacheService.Put("Cache Test", 0);
            _cacheService.Put("Timed Test", 1, new TimeSpan(0, 30, 0));
            _cacheService.Put("Timed Test", 1, new TimeSpan(1, 30, 29));
            _cacheService.Put("Timed Test", 1, new TimeSpan(10,0, 30, 0));
            _cacheService.Put("Timed Test", 1, new TimeSpan(5, 4, 3, 2, 1));
            _cacheService.GetObject<int>("Cache Test");
            _cacheService.GetObject<int>("Cache Miss");
            _cacheService.Remove("Cache Test");
            _cacheService.Clear();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {}
    }
}