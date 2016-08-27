using System;

namespace Orchard.UI.Admin {
    public class AdminAttribute : Attribute {
        public AdminAttribute() {
            Enabled = true;
        }
        public AdminAttribute(bool enabled) {
            Enabled = enabled;
        }

        public bool Enabled { get; set; }
    }
}
