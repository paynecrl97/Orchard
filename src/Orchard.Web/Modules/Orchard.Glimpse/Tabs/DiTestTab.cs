using Autofac;
using Glimpse.Core.Extensibility;
using Orchard.Environment;

namespace Orchard.Glimpse.Tabs {
    public class DiTestTab : TabBase, IKey, ILayoutControl {
        private IOrchardServices _orchardServices;

        public override object GetData(ITabContext context) {
            _orchardServices = OrchardStarter.Container.Resolve<IOrchardServices>();

            return null;
            //return _orchardServices.WorkContext.CurrentSite;
        }

        public override string Name {
            get { return "DI Test"; }
        }

        public string Key {
            get { return "glimpse_orchard_test"; }
        }

        public bool KeysHeadings { get { return false; } }
    }
}