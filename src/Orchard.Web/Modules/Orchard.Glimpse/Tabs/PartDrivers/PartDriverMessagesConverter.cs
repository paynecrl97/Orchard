using System.Collections.Generic;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;
using Orchard.Glimpse.Extensions;

namespace Orchard.Glimpse.Tabs.PartDrivers {
    public class PartDriverMessagesConverter : SerializationConverter<IEnumerable<PartDriverMessage>> {
        public override object Convert(IEnumerable<PartDriverMessage> messages) {
            var root = new TabSection("Content ID", "Content Type", "Content Stereotype", "Content Name", "Part", "Display Type", "Time Taken");
            foreach (var message in messages) {
                root.AddRow()
                    .Column(message.Context.ContentItem.Id)
                    .Column(message.Context.ContentItem.ContentType)
                    .Column(message.Stereotype)
                    .Column(message.Context.ContentItem.GetContentName())
                    .Column(message.Context.ContentPart.PartDefinition.Name)
                    .Column(message.Context.DisplayType)
                    .Column(message.Duration.ToTimingString());
            }

            root.AddTimingSummary(messages);

            return root.Build();
        }
    }
}