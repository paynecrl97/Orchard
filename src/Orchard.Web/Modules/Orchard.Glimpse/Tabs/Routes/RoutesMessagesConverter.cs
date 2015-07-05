using System.Collections.Generic;
using System.Linq;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;
using Orchard.Glimpse.Extensions;

namespace Orchard.Glimpse.Tabs.Routes {
    public class RoutesMessagesConverter : SerializationConverter<IEnumerable<RoutesMessage>> {
        public override object Convert(IEnumerable<RoutesMessage> messages) {
            var root = new TabSection("Area", "Name", "Url", "Data", "Constraints", "Data Tokens", "Priority");
            foreach (var message in messages.OrderByDescending(m => m.Priority)) {
                root.AddRow()
                    .Column(message.Area)
                    .Column(message.Name)
                    .Column(message.Url)
                    .Column(message.Data)
                    .Column(message.Constraints)
                    .Column(message.DataTokens)
                    .Column(message.Priority);
            }

            return root.Build();
        }
    }
}