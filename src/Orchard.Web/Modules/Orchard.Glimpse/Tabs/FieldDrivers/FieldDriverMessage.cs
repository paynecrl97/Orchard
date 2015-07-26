using System;
using Glimpse.Core.Message;
using Orchard.ContentManagement.Handlers;
using Orchard.Glimpse.Models;

namespace Orchard.Glimpse.Tabs.FieldDrivers {
    public class FieldDriverMessage : MessageBase, IDurationMessage
    {
        public string Stereotype { get; set; }
        public BuildDisplayContext Context { get; set; }
        public TimeSpan Duration { get; set; }
    }
}