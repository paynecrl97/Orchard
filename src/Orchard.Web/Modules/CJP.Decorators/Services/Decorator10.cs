using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace CJP.Decorators.Services {
    [OrchardDecorator(Priority)]
    [OrchardFeature("CJP.Decorators.Decorator10")]
    public class Decorator10 : IDecoratorTest
    {
        private const string Priority = "10.0.0";

        private readonly INotifier _notifier;
        private readonly IDecoratorTest _decoratedService;

        public Decorator10(INotifier notifier, IDecoratorTest decoratedService)
        {
            _notifier = notifier;
            _decoratedService = decoratedService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Test()
        {
            _notifier.Add(NotifyType.Information, T("This message is from a decorator with priority {0}", Priority));
            _decoratedService.Test();
        }
    }
}