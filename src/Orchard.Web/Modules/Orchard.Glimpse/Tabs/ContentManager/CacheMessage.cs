using System;
using Glimpse.Core.Message;
using Orchard.ContentManagement;

namespace Orchard.Glimpse.Tabs.ContentManager {
    public class CacheMessage : MessageBase {
        public string Action { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public string Result { get; set; }
        public TimeSpan? ValidFor { get; set; }
        public TimeSpan Duration { get; set; }
    }
}