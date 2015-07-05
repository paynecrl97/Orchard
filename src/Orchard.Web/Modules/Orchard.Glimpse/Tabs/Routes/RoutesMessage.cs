using Glimpse.Core.Message;

namespace Orchard.Glimpse.Tabs.Routes {
    public class RoutesMessage : MessageBase {
        public string Area { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public object Data { get; set; }
        public object Constraints { get; set; }
        public object DataTokens { get; set; }
        public int Priority { get; set; }
    }
}