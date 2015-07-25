using System.Linq;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;

namespace Orchard.Glimpse.Tabs.Tokens {
    public class TokensTab : TabBase, ITabSetup, IKey, ILayoutControl {
        public override object GetData(ITabContext context) {
            var messages = context.GetMessages<TokenMessage>().ToList();

            if (!messages.Any()) {
                return "There have been no Token events recorded. If you think there should have been, check that the 'Glimpse for Orchard Tokens' feature is enabled.";
            }

            return messages;
        }

        public override string Name {
            get { return "Tokens"; }
        }

        public void Setup(ITabSetupContext context) {
            context.PersistMessages<TokenMessage>();
        }

        public string Key {
            get { return "glimpse_orchard_tokens"; }
        }

        public bool KeysHeadings { get { return false; } }
    }
}