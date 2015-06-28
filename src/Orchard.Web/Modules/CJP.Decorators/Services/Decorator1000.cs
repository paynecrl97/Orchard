using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace CJP.Decorators.Services {
    [OrchardDecorator(Priority)]
    [OrchardFeature("CJP.Decorators.Decorator1000")]
    public class Decorator1000 : IDecoratorTest
    {
        private const string Priority = "1000.0.0";

        private readonly INotifier _notifier;
        private readonly IDecoratorTest _decoratedService;

        public Decorator1000(INotifier notifier, IDecoratorTest decoratedService)
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