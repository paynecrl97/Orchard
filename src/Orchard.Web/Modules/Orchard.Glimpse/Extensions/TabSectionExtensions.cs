using System.Collections.Generic;
using System.Linq;
using Glimpse.Core.Tab.Assist;
using Orchard.Glimpse.Models;

namespace Orchard.Glimpse.Extensions {
    public static class TabSectionExtensions {
        public static void AddTimingSummary(this TabSection section, IEnumerable<IDurationMessage> messages) {
            if (!section.Rows.Any()) {
                return;
            }

            var columnCount = section.Rows.Last().Columns.Count();

            var row = section.AddRow();

            for (int i = 0; i < columnCount-2; i++) {
                row.Column("");
            }

            row.Column("Total time:");
            row.Column(messages.Sum(m => m.Duration.TotalMilliseconds).ToTimingString());
            row.Selected();
        }
    }
}