using System.Linq;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;

namespace Orchard.Glimpse.Tabs.PartDrivers {
    public class PartDriversTab : TabBase, ITabSetup, IKey, ILayoutControl {
        public override object GetData(ITabContext context) {
            var messages = context.GetMessages<PartDriverMessage>().ToList();

            if (!messages.Any()) {
                return "There have been no Content Part Driver events recorded. If you think there should have been, check that the 'Glimpse for Orchard Content Part Drivers' feature is enabled.";
            }

            return messages;
        }

        public override string Name {
            get { return "Drivers (Parts)"; }
        }

        public void Setup(ITabSetupContext context) {
            context.PersistMessages<PartDriverMessage>();
        }

        public string Key {
            get { return "glimpse_orchard_partdrivers"; }
        }

        public bool KeysHeadings { get { return false; } }
    }
}