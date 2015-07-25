using System.Collections.Generic;
using System.Linq;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;
using Orchard.Glimpse.Extensions;

namespace Orchard.Glimpse.Tabs.Tokens {
    public class TokenMessagesConverter : SerializationConverter<IEnumerable<TokenMessage>> {
        public override object Convert(IEnumerable<TokenMessage> messages) {
            var root = new TabSection("Target", "Data", "Values", "Time Taken");
            foreach (var message in messages) {
                root.AddRow()
                    .Column(message.Context.Target)
                    .Column(message.Context.Data)
                    .Column(message.Context.Values)
                    .Column(message.Duration.ToTimingString())
                    .WarnIf(!message.Context.Values.Any());
            }

            root.AddTimingSummary(messages);

            return root.Build();
        }
    }
}