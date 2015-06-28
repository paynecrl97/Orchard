﻿using System;
using System.Collections.Generic;
using System.Linq;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;
using Orchard.Glimpse.Extensions;

namespace Orchard.Glimpse.Tabs.ContentManager {
    public class CacheMessagesConverter : SerializationConverter<IEnumerable<CacheMessage>> {
        public override object Convert(IEnumerable<CacheMessage> messages) {
            var root = new TabSection("Action", "Valid For", "Key", "Result", "Value", "Time Taken");
            foreach (var message in messages) {
                root.AddRow()
                    .Column(message.Action)
                    .Column(message.ValidFor.HasValue ? message.ValidFor.Value.ToReadableString() : null)
                    .Column(message.Key)
                    .Column(message.Result)
                    .Column(message.Value)
                    .Column(message.Duration.ToTimingString());
            }

            root.AddRow()
                .Column("")
                .Column("")
                .Column("")
                .Column("")
                .Column("Total time:")
                .Column(messages.Sum(m => m.Duration.TotalMilliseconds).ToTimingString())
                .Selected();

            return root.Build();
        }
    }
}