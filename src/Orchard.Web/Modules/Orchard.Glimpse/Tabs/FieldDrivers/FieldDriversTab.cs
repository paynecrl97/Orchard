using System.Linq;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;

namespace Orchard.Glimpse.Tabs.FieldDrivers {
    public class FieldDriversTab : TabBase, ITabSetup, IKey, ILayoutControl {
        public override object GetData(ITabContext context) {
            var messages = context.GetMessages<FieldDriverMessage>().ToList();

            if (!messages.Any()) {
                return "There have been no Content Part Field events recorded. If you think there should have been, check that the 'Glimpse for Orchard Content Part Fields' feature is enabled.";
            }

            return messages;
        }

        public override string Name {
            get { return "Drivers (Fields)"; }
        }

        public void Setup(ITabSetupContext context) {
            context.PersistMessages<FieldDriverMessage>();
        }

        public string Key {
            get { return "glimpse_orchard_partfields"; }
        }

        public bool KeysHeadings { get { return false; } }
    }
}