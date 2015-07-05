using System.Linq;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;

namespace Orchard.Glimpse.Tabs.Routes {
    public class RoutesTab : TabBase, ITabSetup, IKey {

        public override object GetData(ITabContext context) {
            var messages = context.GetMessages<RoutesMessage>().ToList();

            if (!messages.Any()) {
                return "There have been no Alias or Route events recorded. If you think there should have been, check that the 'Glimpse for Orchard Routes' feature is enabled.";
            }

            return messages;
        }

        public override string Name {
            get { return "Alias & Routes"; }
        }

        public void Setup(ITabSetupContext context) {
            context.PersistMessages<RoutesMessage>();
        }

        public string Key {
            get { return "glimpse_orchard_routes"; }
        }
    }
}