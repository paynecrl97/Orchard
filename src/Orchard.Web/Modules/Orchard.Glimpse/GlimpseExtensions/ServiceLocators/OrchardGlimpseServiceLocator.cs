using Autofac;
using Glimpse.AspNet;
using Glimpse.Core.Framework;
using Orchard.Environment;
using System.Web.Mvc;
using Orchard.ContentManagement;
using System;
using System.Web;

namespace Orchard.Glimpse.GlimpseExtensions.ServiceLocators {
    public class OrchardGlimpseServiceLocator : AspNetServiceLocator, IServiceLocator, IGlimpseShim {

        public IOrchardServices OrchardServices{ get; set; }

        public new T GetInstance<T>() where T : class {

            var context = HttpContext.Current.Request.RequestContext.GetWorkContext();
            if (context != null)
            {
                var services = context.Resolve<IOrchardServices>();
            }

            OrchardServices = DependencyResolver.Current.GetService<IOrchardServices>();

            //var cm = services.WorkContext.Resolve<IContentManager>();

            if (OrchardServices != null) {
                var x = OrchardServices.WorkContext.Resolve<IContentManager>();
            }


            return base.GetInstance<T>();
        }
    }

    public interface IGlimpseShim : IDependency
    {
        IOrchardServices OrchardServices { get; set; }
    }
}