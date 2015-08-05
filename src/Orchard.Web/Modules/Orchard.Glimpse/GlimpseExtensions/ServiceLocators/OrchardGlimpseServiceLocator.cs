using Autofac;
using Glimpse.AspNet;
using Glimpse.Core.Framework;
using Orchard.Environment;

namespace Orchard.Glimpse.GlimpseExtensions.ServiceLocators {
    public class OrchardGlimpseServiceLocator : AspNetServiceLocator, IServiceLocator {

        public new T GetInstance<T>() where T : class {
            var container = OrchardStarter.Container;

            T instance;
            if (container != null && container.TryResolve(out instance)) {
                return instance;
            }

            return base.GetInstance<T>();
        }
    }
}