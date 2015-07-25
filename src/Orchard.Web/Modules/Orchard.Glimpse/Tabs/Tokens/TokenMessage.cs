using System;
using Glimpse.Core.Message;
using Orchard.Glimpse.Models;
using Orchard.Tokens;

namespace Orchard.Glimpse.Tabs.Tokens {
    public class TokenMessage : MessageBase, IDurationMessage {
        public EvaluateContext Context { get; set; }
        public TimeSpan Duration { get; set; }
    }
}