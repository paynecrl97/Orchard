﻿using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using System.Linq;

namespace Orchard.Glimpse.Tabs.ContentManager {
    public class CacheTab : TabBase, ITabSetup, IKey, ILayoutControl {
        public override object GetData(ITabContext context) {
            var messages = context.GetMessages<CacheMessage>().ToList();

            if (!messages.Any()) {
                return "There have been no Cache events recorded. If you think there should have been, check that the 'Glimpse for Orchard Cache Service' feature is enabled.";
            }

            return messages;
        }

        public override string Name {
            get { return "Cache"; }
        }

        public void Setup(ITabSetupContext context) {
            context.PersistMessages<CacheMessage>();
        }

        public string Key {
            get { return "glimpse_orchard_cache"; }
        }

        public bool KeysHeadings { get { return false; } }
    }
}