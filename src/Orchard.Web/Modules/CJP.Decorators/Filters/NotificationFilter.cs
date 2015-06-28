using System.Web.Mvc;
using CJP.Decorators.Services;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;
using Orchard.UI.Admin;

namespace CJP.Decorators.Filters
{
    public class NotificationFilter : FilterProvider, IActionFilter
    {
        private readonly IDecoratorTest _decoratorTest;

        public NotificationFilter(IDecoratorTest decoratorTest)
        {
            _decoratorTest = decoratorTest;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!AdminFilter.IsApplied(filterContext.RequestContext))
            {
                _decoratorTest.Test();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}