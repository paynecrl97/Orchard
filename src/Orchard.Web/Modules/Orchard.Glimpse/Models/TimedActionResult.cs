using Glimpse.Core.Extensibility;

namespace Orchard.Glimpse.Models {
    public class TimedActionResult<T> {
        public TimerResult TimerResult { get; set; }
        public T ActionResult { get; set; }
    }
}